# Prima - A Modern Ultima Online Server Implementation

[//]: # (![Prima Logo]&#40;logo.png&#41;)

## Overview

Prima is a modern, high-performance implementation of an Ultima Online server built with C# and .NET 9. This project aims to revive the classic Ultima Online experience while leveraging contemporary software development practices, extensible architecture, and improved performance.

## Vision

Ultima Online was a groundbreaking MMORPG that pioneered many concepts we take for granted in online gaming today. Prima builds upon this legacy by:

- Reimagining the server architecture with modern design patterns
- Utilizing the latest .NET features for improved performance
- Creating a clean, extensible codebase that's easy to understand and modify
- Supporting both classic and modern UO clients
- Providing robust APIs for custom content creation

## Project Structure

- **Prima.Network**: Core networking components, packet definitions, serializers, and protocol implementations
- **Prima.Server**: Main server application handling game state, world simulation, and client connections
- **Prima.Tests**: Comprehensive test suite ensuring reliability and correctness

## Features

- **Modern C# Implementation**: Built with the latest C# features and .NET 9
- **High Performance**: Optimized for efficiency with modern hardware
- **Extensible Architecture**: Modular design allowing for easy customization
- **Protocol Compatibility**: Full support for the UO network protocol
- **Containerization Ready**: Docker support for easy deployment
- **Comprehensive Testing**: Thorough test coverage for reliability

## Development Status

Prima is currently in early development. The networking layer and packet structures are being implemented, with core game functionality to follow.

## Getting Started

### Prerequisites

- .NET 9 SDK
- Docker (optional, for containerized deployment)

### Building the Project

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Running the Server

```bash
dotnet run --project src/Prima.Server/Prima.Server.csproj
```

### Docker Support

Build the Docker image:

```bash
docker build -t prima-server -f src/Prima.Server/Dockerfile .
```

Run the container:

```bash
docker run -p 2593:2593 prima-server
```

## Contributing

Contributions are welcome! If you'd like to contribute, please:

1. Fork the repository
2. Create a feature branch
3. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- The Ultima Online team for creating a groundbreaking MMORPG
- The open-source community for their invaluable tools and libraries
- All contributors to the project

---

*Prima is not affiliated with Electronic Arts or the official Ultima Online game. This is a fan project created to explore modern game server implementations while honoring the legacy of Ultima Online.*
