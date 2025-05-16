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
- 🌍 Multi-language support with localization services
- 💾 Automatic world saving system
- 🧰 Modern UOP file format support

## Scripting System

PrimaUO includes a powerful JavaScript scripting engine that allows you to extend and customize server functionality without modifying the core code. Scripts are loaded from the `scripts` directory and can interact with server systems through registered modules.

### Example Script

```javascript
// bootstrap.js
// This script is executed when the server starts

// Import script modules are automatically registered by the server
// Log a message to the server console
console.Log("Server startup script loaded!");

// Listen for server started event
events.OnStarted(() => {
    logger.Info("Server has started successfully!");

    // Register admin commands
    commands.RegisterConsoleCommand(
        "announce",
        "Broadcasts a message to all players",
        (args) => {
            const message = args.join(" ");
            logger.Info(`Broadcasting message: ${message}`);
            // Actual broadcast implementation would go here
            return true;
        },
        ["broadcast", "shout"],
        CommandType.ALL,
        CommandPermissionType.ADMIN
    );

    // Create a recurring task to run every 5 minutes
    scheduler.ScheduleTask(
        "cleanup",
        300,  // seconds
        () => {
            logger.Info("Running scheduled cleanup task...");
            // Cleanup logic would go here
        }
    );

    // Register an event handler for character creation
    events.OnCharacterCreated((args) => {
        logger.Info(`New character created: ${args.Name}`);

        // Start a delayed welcome timer
        timers.OneShot("welcome_message", 10, () => {
            logger.Info(`Sending welcome message to ${args.Name}`);
            // Actual welcome message code would go here
        });
    });

    // Register custom item
    items.Register("enchanted_sword", {
        ItemId: 3937,
        Name: "Enchanted Longsword",
        GraphicId: 0x0F60,
        Weight: 4.0,
        Layer: Layer.ONE_HANDED,
        Amount: 1,
        Damage: {
            Min: 10,
            Max: 25,
            Type: "Slashing"
        }
    });
});
```

### Available Script Modules

The scripting API exposes the following modules:

- **logger**: Functions for logging at different levels (Info, Debug, Warn, Error, Critical)
- **events**: Subscribe to server events like OnStarted, OnUserLogin, OnCharacterCreated
- **scheduler**: Schedule tasks to run periodically
- **template**: Add variables for template rendering and text processing
- **files**: Include other script files or directories
- **timers**: Create one-time or repeating timers with delays
- **items**: Register custom items with properties and behaviors
- **console**: Log messages to the console or prompt for input
- **commands**: Register custom commands for console or in-game use

## Project Structure

- **Prima.Core.Server**: Core server infrastructure
- **Prima.Network**: Network layer and packet handling
- **Prima.Server**: Main server application
- **Prima.UOData**: Ultima Online data structures and utilities
- **Prima.JavaScript.Engine**: JavaScript scripting engine for server extensibility
- **Prima.Tcp.Test**: TCP testing utilities

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Ultima Online client files for data loading

### Configuration

Configuration is handled through YAML files located in the `configs` directory. Here's an example configuration:

```yaml
debug:
  save_raw_packets: false
process:
  pid_file: server.pid
  process_queue:
    max_parallel_task: 2
shard:
  max_players: 1000
  uo_directory: /path/to/uo/files
  client_version:
  name: Prima Shard
  admin_email: admin@primauo.com
  url: https://github.com/tgiachi/prima
  time_zone: 0
  language: en
  autosave:
    interval_in_minutes: 5
jwt_auth:
  issuer: Prima
  audience: Prima
  secret: YourSecretKey
  expiration_in_minutes: 44640
  refresh_token_expiry_days: 1
accounts:
  is_registration_enabled: true
smtp:
  host: localhost
  port: 25
  username: ''
  password: ''
  use_ssl: false
  use_start_tls: false
tcp_server:
  host: ''
  login_port: 2593
  game_port: 2592
  enable_web_server: true
  web_server_port: 23000
  log_packets: true
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

## Type Definitions for Scripts

The server automatically generates TypeScript definitions for the scripting API in `scripts/index.d.ts`, which provides autocomplete and type checking when using an editor like Visual Studio Code:

```typescript
/**
 * prima v0.15.4.0 JavaScript API TypeScript Definitions
 * Auto-generated documentation on 2025-05-16 10:46:15
 **/

// Constants
declare const APP_NAME: string;
declare const VERSION: string;

// Modules
declare const logger: {
    Info(Message: string, Args: any[]): void;
    Warn(Message: string, Args: any[]): void;
    Error(Message: string, Args: any[]): void;
    Critical(Message: string, Args: any[]): void;
    Debug(Message: string, Args: any[]): void;
};

declare const events: {
    OnStarted(Action: () => void): void;
    HookEvent(EventName: string, EventHandler: (arg: any) => void): void;
    OnUserLogin(Action: (arg: IUserLoginContext) => void): void;
    OnCharacterCreated(Action: (arg: ICharacterCreatedEventArgs) => void): void;
};

// ... and other module definitions
```

### Help Wanted!
I'm actively seeking developers to join the PrimaUO project!
This project aims to modernize Ultima Online and keep this legendary MMORPG alive for future generations. Whether you're skilled in C#, JavaScript, network programming, or game development, your contributions can make a difference.
Why join us:

Work on preserving gaming history
Modernize a classic MMORPG with cutting-edge technologies
Join a passionate community of UO enthusiasts
Gain experience with .NET 9.0, modern architecture patterns, and game server development

No previous Ultima Online development experience is required - just enthusiasm and a willingness to learn. If you're interested in contributing, please reach out via GitHub issues or discussions.
Together, we can ensure Ultima Online continues to thrive for years to come!


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
- [UOX3](https://github.com/UOX3DevTeam/UOX3) for inspiration
