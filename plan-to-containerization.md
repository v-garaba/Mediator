Plan: Containerize .NET 10 Solution with Podman
Containerize the three executable projects (Mediators console app, Mediator.Chat.Api, Mediator.Users.Api) using Podman with multi-stage Dockerfiles and Podman Compose for orchestration.

Steps
Create .dockerignore at solution root to exclude bin/, obj/, and other build artifacts from the container build context.

Create Mediator.Users.Api/Dockerfile — Multi-stage build using mcr.microsoft.com/dotnet/sdk:10.0-preview for building and mcr.microsoft.com/dotnet/aspnet:10.0-preview for runtime, exposing port 8080.

Create Mediator.Chat.Api/Dockerfile — Same pattern as Users API, exposing port 8080 inside the container.

Create Mediators/Dockerfile — Multi-stage build using mcr.microsoft.com/dotnet/runtime:10.0-preview for runtime (console app, no web server).

Create podman-compose.yml at solution root — Define three services (users-api, chat-api, mediators), configure container networking so mediators can reach APIs via service names (http://users-api:8080, http://chat-api:8080), add health checks and depends_on for startup order.

Run containerized stack — Use podman-compose up --build to build images and start all containers.

Further Considerations
.NET 10 is preview — Base images use 10.0-preview tags; ensure Podman can pull these or use nightly tag. Do you have .NET 10 SDK installed locally for verification?

Port mapping — Should the APIs be accessible from the host at the original ports (61795/61796) or different ports?

Persistent data — The current implementation stores data in-memory; do you need volume mounts for any persistent storage later?