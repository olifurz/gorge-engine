<!-- Copilot / AI agent instructions for Nekoblocks (gorge-engine) -->

# Nekoblocks — Quick AI Coding Guide

This repository is a small .NET 8 game engine built with Raylib (raylib-cs) and Jitter2. Use these notes to get up to speed quickly and make safe, consistent changes.

- **Run / build**: Requires .NET 8 SDK. From repo root run:
  - `dotnet run --project nekoblocks.csproj` (or `dotnet run`).

- **Big picture / architecture**:
  - Core loop: `Program.cs` initializes Raylib then calls `ServiceManager.Initialise()` and runs `ServiceManager.Update()` each frame.
  - Services: everything important is a service deriving from `Nekoblocks.Core.BaseService` and registered in `Nekoblocks.Core.ServiceManager`.
    - Current startup order (order matters): `GameService`, `ResourceService`, `WorkspaceService`, `PhysicsService`, `RenderService`.
  - Scene graph: `Nekoblocks.Game.Instance` is the node type (parent/children). `GameService.Root` is the root instance. Use `SetParent`/`GetChildren` to manipulate hierarchy.

- **Key subsystems & files** (read these before changing related code):
  - Service manager & lifecycle: `src/Nekoblocks/Nekoblocks.Core/ServiceManager.cs`, `BaseService.cs`
  - Game/scene objects: `src/Nekoblocks/Nekoblocks.Game/Instance.cs`
  - Workspace composition: `src/Nekoblocks/Nekoblocks.Services/WorkspaceService.cs`
  - Rendering: `src/Nekoblocks/Nekoblocks.Services/RenderService.cs` (uses `Player.Camera`)
  - Physics: `src/Nekoblocks/Nekoblocks.Services/PhysicsService.cs` (Jitter2 world)
  - Parts/Models: `src/Nekoblocks/Nekoblocks.Game.Objects/Part.cs` (mesh + shader setup)
  - Player/Character: `src/Nekoblocks/Nekoblocks.Game.Player/Player.cs` and `Character.cs`
  - Resource loading: `src/Nekoblocks/Nekoblocks.Services/ResourceService.cs`

- **Project-specific conventions & important details**:
  - Service ordering is significant. If you add a service that other services use in `Start()`, register it before dependents.
  - Resources are embedded and loaded via `ResourceService.GetResource("<dot.separated.path>")`.
    - Example: `textures.stud.png` becomes `assemblyName.assets.textures.stud.png` internally. Use dot-separated paths (e.g. `shaders.surface.fs`).
  - `Instance` IDs come from `GameService.GetUniqueId()`; code often refers to objects by instance id.
  - `Transform` (namespace `Nekoblocks.World`, at `src/Nekoblocks/Nekoblocks.Game/Transform.cs`) uses Euler angles in **degrees**. Helper methods convert to/from quaternions and axis-angle. Events (`PositionChanged`, `RotationChanged`, `ScaleChanged`) are invoked when values change, allowing reactive updates to physics/rendering.
  - `Part` (derived from `Instance`) creates its physics `RigidBody` in its constructor via `PhysicsService.AddBody(this)`. The constructor also wires `Transform` events to physics updates. Do not reorder Part construction.
  - `RenderService` only draws `Part` instances with `Transparency < 1` and uses `Player.Camera` for the 3D view.
  - Camera control: `Player` uses spherical coords (`theta`, `phi`) with mouse-right drag to rotate; movement is applied via `Character`/physics forces.
  - Unsafe code is used sparingly: Raylib interop in `Program.cs` for log callback setup, shader loading in `ResourceService` for memory management, and material setup in `Part.RegenerateModel()`. Use `fixed` statements when working with pointer conversions.

- **Resource / shader examples**:
  - Skybox created in `WorkspaceService` with `new Skybox("textures.skybox.png", false)` (see `Nekoblocks.Game.Objects.Skybox`).
  - Surface shader is loaded from resource path `shaders.surface.fs` and expects a `tiling` uniform (vec2).

- **Debugging and logs**:
  - Raylib logs are forwarded into the project logger via `Log.RaylibLogCallback` (see `Nekoblocks.Utilities.Log`). Use `Log.Info/Debug/Warning/Error/Critical`.
  - To inspect the scene tree at runtime, inspect `GameService.Root` and `WorkspaceService.Workspace`.
  - The logger automatically captures the calling class from the stack trace.

- **Adding a new Service** (most common extension point):
  1. Create a class deriving from `BaseService` in `src/Nekoblocks/Nekoblocks.Services/`.
  2. Register it in `ServiceManager.Initialise()` **after** any services it depends on. Override `Start()`, `Update()`, and `Stop()` as needed.
  3. Access other services via `ServiceManager.GetService<T>()`.
  4. **Critical**: Services registered later can depend on earlier ones, but not vice-versa.

- **Safe change patterns**:
  - When changing creation order (services, instances, or resource loading), update `ServiceManager.Initialise()` and verify Start() ordering.
  - Prefer adding features via new services rather than expanding existing ones when possible.
  - Keep `Part` and physics creation behavior consistent: creating a `Part` currently assumes immediate physics body creation.

If anything here is unclear or you'd like me to expand examples (e.g., how to add a new service or wire a shader), tell me which section to expand.
