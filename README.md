# PositionEvents
Provides functionality to efficiently bundle position checking. Intended for [Blish-Hud](https://github.com/blish-hud/Blish-HUD) modules.

The main `PositionHandler` currently uses an Octree implementation.

## Using PositionEvents with your Blish-Hud module
> [!WARNING]
> You should not directly use this library inside your module. Please use the provided
> [Position Events Module](https://github.com/Flyga-M/PositionEventsModule) as
> a dependency instead. It's more efficient if multiple modules use the same `PositionHandler`. The module also
> provides some debug functionality on top.

1. Add the PositionEvents Package as a reference to your module. It is available as a [NuGet](https://www.nuget.org/packages/PositionEvents) package.
2. Add the Position Events Module [.dll](https://github.com/Flyga-M/PositionEventsModule/releases/) as a reference to your module.
3. Add the Position Events Module as a dependency to your module manifest.
4. Retrieve a reference to the Position Events Module instance during your `LoadAsync` method.
5. Register your desired areas with the Position Events Module.

> [!TIP]
> Make sure that the references of the PositionEvents Package and the Position Events Module have **Copy Local** set
> to **False** in the properties of the reference. You don't need to ship them with your module, since they will
> already be present, because of your modules dependence on the Position Events Module.