import json
import os
import shutil
import subprocess
from typing import Iterable, Optional

from pyxtension.streams import stream
from openapi_typed import OpenAPIObject


def find_model(search_dirs: Iterable[str], name: str) -> Optional[str]:
    for search_dir in search_dirs:
        parent_dir = os.path.abspath(os.path.join(search_dir, '..'))
        model_dir = os.path.join(search_dir, 'Model')
        for root, dirs, files in os.walk(model_dir):
            for file in files:
                if file.endswith('.cs') and file[:-3] == name:
                    base_dir = os.path.relpath(root, parent_dir)
                    return base_dir.replace(os.path.sep, '.')
    
    return None


def find_schema(import_mappings: dict[str, str], schema_name: str, search_dirs: list[str]) -> Optional[str]:
    existing = import_mappings.get(schema_name, None)
    if existing is not None:
        return existing
        
    mapping = find_model(search_dirs, schema_name)
    if mapping is not None:
        import_mappings[schema_name] = mapping
        return mapping
    
    return None

def fix_dotnet_client(out_dir: str, import_mappings: dict[str, str]):
    base_dir = os.path.join(out_dir, 'src', 'UnrealPluginManager.WebClient')
    api_dir = os.path.join(base_dir, 'Api')
    namespaces = '\n'.join(stream(import_mappings.values()).map(lambda x: f'using {x};').distinct())
    page_types = '\n'.join(stream(import_mappings.keys())
                           .filter(lambda x: x.endswith('Page'))
                           .map(lambda x: f'    using {x} = Page<{x[:-4]}>;'))
    for root, dirs, files in os.walk(api_dir):
        for file in files:
            if file.endswith('.cs'):
                
                with open(os.path.join(root, file), 'r+') as f:
                    content = f.read()
                    content = content.replace('using UnrealPluginManager.WebClient.Model;', namespaces)
                    content = content.replace('namespace UnrealPluginManager.WebClient.Api\n{', 
                                              f'namespace UnrealPluginManager.WebClient.Api\n{{\n{page_types}')
                    f.seek(0)
                    f.write(content)
                    f.truncate()
    
    models_dir = os.path.join(base_dir, 'Model')
    for root, dirs, files in os.walk(models_dir):
        for file in files:
            if file.endswith('.cs') and file != 'AbstractOpenAPISchema.cs':
                os.remove(os.path.join(root, file))
                
                
def move_dotnet_client(out_dir: str, root_dir: str):
    base_dir = os.path.join(out_dir, 'src', 'UnrealPluginManager.WebClient')
    generated_dir = os.path.join(root_dir, 'UnrealPluginManager.WebClient')
    api_dir = os.path.join(generated_dir, 'Api')
    client_dir = os.path.join(generated_dir, 'Client')
    models_dir = os.path.join(generated_dir, 'Model')
    os.makedirs(generated_dir, exist_ok=True)
    shutil.rmtree(api_dir, ignore_errors=True)
    shutil.rmtree(client_dir, ignore_errors=True)
    shutil.rmtree(models_dir, ignore_errors=True)
    os.rename(os.path.join(base_dir, 'Api'), api_dir)
    os.rename(os.path.join(base_dir, 'Client'), client_dir)
    os.rename(os.path.join(base_dir, 'Model'), models_dir)
    shutil.rmtree(out_dir)
    
    
def main():
    script_dir = os.path.dirname(__file__)
    with open(os.path.join(os.path.join(script_dir, 'openapi-spec.json'))) as f:
        spec: OpenAPIObject = json.load(f)

    root_dir = os.path.abspath(os.path.join(script_dir, '..'))
    search_dirs = [os.path.join(root_dir, 'UnrealPluginManager.Core')]
    import_mappings = {}
    schemas = spec['components']['schemas']
    for schema_name, schema in schemas.items():
        if schema_name in import_mappings:
            continue
            
        if schema_name.endswith('Page'):
            import_mappings[schema_name] = 'UnrealPluginManager.Core.Pagination'
        else:
            find_schema(import_mappings, schema_name, search_dirs)
            
    config_file = os.path.join(script_dir, 'openapitools.json')
    out_dir = os.path.join(root_dir, 'generated', 'dotnet')
    commands = ['openapi-generator-cli', 'generate', '--generator-name', 'csharp', 
                '--config', config_file,
                '--input-spec', os.path.join(script_dir, 'openapi-spec.json'),
                '--output', out_dir,
                '--package-name', 'UnrealPluginManager.WebClient',]
    subprocess.call(commands, shell=True)
    fix_dotnet_client(out_dir, import_mappings)
    move_dotnet_client(out_dir, root_dir)

if __name__ == '__main__':
    main()