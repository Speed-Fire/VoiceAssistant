# Voice Assitant

Voice Assitant is a modular application for voice-controlled computer interaction. It allows users to extend functionality through plugins and visually create script logic similar to Unreal Engine's Blueprints.

⚠️ This application is under active development. Script creation and compilation are not fully implemented yet.

## Features

- 🎙️ Voice command recognition with support for parameter extraction.
- 🧩 Plugin system to expand capabilities:
  - Speech-to-text service providers.
  - Command meaning parsers.
  - Script functionality enhancements.
- 🎛️ Visual editor for creating scripts using nodes and links.
- ⚙️ Script generation in C# with on-the-fly compilation using Roslyn.

## Interface Preview

![Voice Assistant](https://github.com/user-attachments/assets/e16e6cc2-fd74-472c-87f1-ea9e78cddf88)

## Plugin Support

You can extend the application through external plugins that follow a specific interface. These plugins can add:
- New speech recognition services.
- Natural language understanding modules.
- Custom nodes for the script editor.

Plugin loading is dynamic and allows for future integration with cloud-based services or advanced AI modules.

## Tech Stack

- C# / .NET
- Roslyn Compiler
- REST API
- Machine Learning
- Custom visual scripting engine
- Windows platform (initial support)

## Icons & Licensing

Some icons used in this project are sourced from [Flaticon](https://www.flaticon.com/).  
Unfortunately, the original author names were lost. If you recognize any of your work in the project, please contact us so we can credit you properly.

## Roadmap

- [x] Common UI and bussiness logic
- [x] Plugin manager UI
- [x] Plugin system
- [x] Basic voice recognition and command parsing
- [ ] Complete visual script editor
- [ ] Full script-to-C# translation
- [ ] Roslyn compilation integration
- [ ] Documentation and plugin development guide

## License

This project is released under the MIT License.
