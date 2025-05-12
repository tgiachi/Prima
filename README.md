# PrimaUO Server

![Docker Image Version (tag)](https://img.shields.io/docker/v/tgiachi/prima-server/latest)
[![GitHub Actions](https://github.com/tgiachi/prima/actions/workflows/docker_image.yml/badge.svg)](https://github.com/tgiachi/prima/actions/workflows/docker_image.yml)
[![License](https://img.shields.io/badge/license-MIT-red.svg)](LICENSE)

![](./Assets/prima_logo.png)

PrimaUO is a modern Ultima Online server implementation written in C# (.NET 9.0). The project aims to provide a modern, extensible, and maintainable server alternative with support for the latest client versions.

## Features

- ⚡ Modern C# (.NET 9.0) implementation
- 🔄 Event-driven architecture
- 🚀 High performance through optimized code and event loop system
- 🔌 Modular design with dependency injection
- 📦 Support for multiple client versions
- 🔒 Secure authentication using JWT
- 📈 Built-in metrics and diagnostics
- 🔧 Extensible through JavaScript scripting system
- 🌐 REST API for integration with external tools

## Scripting System

PrimaUO includes a powerful JavaScript scripting engine that allows you to extend and customize server functionality without modifying the core code. Scripts are loaded from the `scripts` directory and can interact with server systems through registered modules.

### Example Script

```javascript
// server_startup.js
// This script is executed when the server starts

// Import script modules
const events = require("events");
const commands = require("commands");
const timers = require("timers");
const scheduler = require("scheduler");

// Listen for server started event
events.OnStarted(() => {
    console.log("Server has started successfully!");

    // Register admin commands
    commands.RegisterConsoleCommand(
        "announce",
        "Broadcasts a message to all players",
        (args) => {
            // Implementation of the announce command
            const message = args.join(" ");
            console.log(`Broadcasting message: ${message}`);
            // Actual broadcast implementation would go here
            return true;
        },
        ["broadcast", "shout"],
        "Console",
        "Admin"
    );

    // Create a recurring task to run every 5 minutes
    scheduler.ScheduleTask(
        "cleanup",
        300,  // seconds
        () => {
            console.log("Running scheduled cleanup task...");
            // Cleanup logic would go here
        }
    );

    // Register an event handler for character creation
    events.OnCharacterCreated((args) => {
        console.log(`New character created: ${args.Name}`);

        // Start a delayed welcome timer
        timers.OneShot("welcome_message", 10, () => {
            console.log(`Sending welcome message to ${args.Name}`);
            // Actual welcome message code would go here
        });
    });
});
```

### Available Script Modules

- **events**: Subscribe to server events
- **commands**: Register custom commands
- **timers**: Create one-time or repeating timers
- **scheduler**: Schedule tasks to run periodically
- **template**: Add variables for template rendering
- **files**: Include other script files

## Project Structure

- **Prima.Core.Server**: Core server infrastructure
- **Prima.Network**: Network layer and packet handling
- **Prima.Server**: Main server application
- **Prima.UOData**: Ultima Online data structures and utilities
- **Prima.Tcp.Test**: TCP testing utilities

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Ultima Online client files for data loading

### Configuration

Configuration is handled through YAML files located in the `configs` directory:

```yaml
Prima:
  Server:
    Shard:
      Name: "Prima Shard"
      UoDirectory: "/path/to/uo/files"
      MaxPlayers: 1000
    TcpServer:
      Host: ""
      LoginPort: 2593
      GamePort: 2592
      EnableWebServer: true
      WebServerPort: 23000
      LogPackets: true
```

### Building from Source

```bash
git clone https://github.com/tgiachi/prima.git
cd prima
dotnet build
```

### Running the Server

```bash
cd src/Prima.Server
dotnet run
```

### Docker Support

A Dockerfile is provided to build and run the server in a container:

```bash
docker build -t primauo/server .
docker run -p 2593:2593 -p 2592:2592 -p 23000:23000 -v /path/to/uo/files:/app/data/client primauo/server
```

## API Documentation

The server includes a Swagger UI for API documentation available at:

```
http://localhost:23000/swagger
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- The Ultima Online community
- [ModernUO](https://github.com/modernuo/modernuo) for inspiration and some utility code
- [Orion](https://github.com/tgiachi/orion) for the server foundation framework
