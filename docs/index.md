# Orion IRC Server Documentation

![Version](https://img.shields.io/badge/version-0.4.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)

> IRC is not dead, long live IRC!

Welcome to the official documentation for Orion IRC Server, a modern, scalable IRC server built with .NET 9.0.

## Introduction

Orion is designed to provide robust IRC functionality while maintaining high performance and flexibility. It features a modular architecture that makes it easy to extend and customize.

## Getting Started

- [Quick Start Guide](./guides/quick-start.md)
- [Installation](./guides/installation.md)
- [Configuration](./guides/configuration.md)
- [Docker Deployment](./guides/docker.md)

## Core Components

Orion is built as a collection of modular components:

- [**Orion.Foundations**](./components/foundations.md): Base utilities, extensions, and common functionality
- [**Orion.Core.Server**](./components/core-server.md): Server-side core functionality
- [**Orion.Core.Server.Web**](./components/core-server-web.md): Web API and HTTP interface
- [**Orion.Irc.Core**](./components/irc-core.md): IRC protocol implementation
- [**Orion.Network.Core**](./components/network-core.md): Networking abstractions
- [**Orion.Network.Tcp**](./components/network-tcp.md): TCP implementation for network transports

## Advanced Topics

- [Event System](./advanced/event-system.md)
- [Network Transport Architecture](./advanced/network-transport.md)
- [IRC Command Handling](./advanced/command-handling.md)
- [Scripting with JavaScript](./advanced/scripting.md)
- [Text Templating](./advanced/text-templating.md)
- [Security Features](./advanced/security.md)

## Development

- [Building from Source](./development/building.md)
- [Adding New Commands](./development/adding-commands.md)
- [Creating Plugins](./development/creating-plugins.md)
- [Contributing Guidelines](./development/contributing.md)
- [Coding Standards](./development/coding-standards.md)

## API Reference

- [HTTP API Documentation](./api/http-api.md)
- [JavaScript API](./api/javascript-api.md)

## Troubleshooting

- [Common Issues](./troubleshooting/common-issues.md)
- [Logs and Diagnostics](./troubleshooting/logs-diagnostics.md)
- [Performance Tuning](./troubleshooting/performance-tuning.md)

## Community and Support

- [Community Resources](./community/resources.md)
- [Support Channels](./community/support.md)
- [Contributing to Orion](./community/contributing.md)

## Release Notes

- [Version History](./releases/version-history.md)
- [Roadmap](./releases/roadmap.md)

## License

Orion IRC Server is licensed under the MIT License. See the [LICENSE](https://github.com/tgiachi/orion/blob/main/LICENSE) file for details.
