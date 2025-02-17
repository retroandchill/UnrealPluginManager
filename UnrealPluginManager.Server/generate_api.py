import json
import os
import subprocess
from typing import Iterable, Optional

from openapi_typed import OpenAPIObject


def find_model(search_dirs: Iterable[str], name: str) -> Optional[str]:
    for search_dir in search_dirs:
        parent_dir = os.path.abspath(os.path.join(search_dir, '..'))
        model_dir = os.path.join(search_dir, 'Model')
        for root, dirs, files in os.walk(model_dir):
            for file in files:
                if file.endswith('.cs') and file[:-3] == name:
                    base_dir = os.path.relpath(root, parent_dir)
                    namespace = base_dir.replace(os.path.sep, '.')
                    return f'{namespace}.{name}'
    
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
            generic_param = find_schema(import_mappings, schema_name[:-4], search_dirs)
            import_mappings[schema_name] = f'UnrealPluginManager.Core.Pagination.Page<{generic_param}>'
        else:
            find_schema(import_mappings, schema_name, search_dirs)
            
    config_file = os.path.join(script_dir, 'openapitools.json')
    with open(config_file) as f:
        config = json.load(f)

    config['importMappings'] = import_mappings
    config['typeMappings'] = import_mappings
    
    with open(config_file, 'w') as f:
        json.dump(config, f, indent=4)
        

    commands = ['openapi-generator-cli', 'generate', '--generator-name', 'csharp', 
                '--input-spec', os.path.join(script_dir, 'openapi-spec.json'),
                '--output', os.path.join(root_dir, 'generated', 'dotnet'),
                '--package-name',  'UnrealPluginManager.WebClient']
    subprocess.call(commands, shell=True)

if __name__ == '__main__':
    main()