# PositionEvents
Provides functionality to efficiently bundle position checking. Intended for [Blish-Hud](https://github.com/blish-hud/Blish-HUD) modules.

The main `PositionHandler` currently uses an Octree implementation.

## Using PositionEvents with your Blish-Hud module
> [!WARNING]
> You should not directly use this library inside your module. Please use the provided
> [Position Events Module](https://github.com/Flyga-M/PositionEventsModule) as
> a dependency instead. It's more efficient if multiple modules use the same `PositionHandler`. The module also
> provides some debug functionality on top.

Please follow the instructions at the [Position Events Module](https://github.com/Flyga-M/PositionEventsModule).