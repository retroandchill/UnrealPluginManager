$engine_dir = "C:\Program Files\Epic Games\UE_5.5\Engine\Build\BatchFiles"
$current_dir = $(Get-Location)

& "$engine_dir/RunUAT.bat" BuildPlugin -plugin="D:\dev\UnrealProjects\UnrealPokemon\Plugins\External\UE5Coro\UE5Coro.uplugin" -package="$($current_dir)/../data/UE5Coro"