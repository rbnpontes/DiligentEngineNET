# DiligentEngineNET

**DiligentEngineNET** is a .NET Core binding for [DiligentCore](https://github.com/DiligentGraphics/DiligentCore), a powerful cross-platform rendering engine. While Diligent Engine offers C# bindings through its [NuGet package](https://www.nuget.org/packages/DiligentGraphics.DiligentEngine.Core), these bindings currently lack full cross-platform support for Linux, macOS, Android, and Web platforms.

The primary goal of **DiligentEngineNET** is to provide a robust and elegant .NET API for Diligent Engine, while maintaining cross-platform compatibility. We aim to stay faithful to the original Diligent API design, offering intuitive C# interfaces without deviating from the architecture.

### Key Features:

1. **Cross-Platform Support**: This project aims to support Windows, Linux, macOS, Android, and Web.
2. **Elegant .NET API**: The bindings are designed to provide a clean, idiomatic .NET interface while closely mirroring the original Diligent API.
   - All Diligent interfaces are implemented as C# interfaces to provide a familiar and structured development experience.
   - Getters and Setters will be .NET properties
   - 

### Note on Performance:

If your project demands **high performance**, this binding may not be suitable. **DiligentEngineNET** relies on standard interop calls, which may be slower than the virtual calls used in the original Diligent Engine's C# bindings. For performance-critical applications, we recommend using the official [Diligent Engine C# bindings](https://www.nuget.org/packages/DiligentGraphics.DiligentEngine.Core).

---

## Current Status

| Platform | Status      |
| -------- | ----------- |
| Windows  | In Progress |
| Linux    | Not Started |
| macOS    | Not Started |
| Android  | Not Started |
| Web      | Not Started |

---

## Getting Started

*Documentation Coming Soon*

Stay tuned for instructions on how to set up and use **DiligentEngineNET** in your projects. In the meantime, you can explore the official [Diligent Engine documentation](https://github.com/DiligentGraphics/DiligentCore#readme) to familiarize yourself with the API.

---

## License

This project is licensed under the **MIT License** for both the code generator and native/.NET implementations. Please note that **Diligent Engine Core** itself uses the **Apache 2.0 License**. For more details on the licensing of Diligent Engine Core, refer to their [Apache 2.0 License documentation](https://github.com/DiligentGraphics/DiligentCore?tab=Apache-2.0-1-ov-file#readme).

---

## Contributing

Contributions are welcome! If you would like to contribute, please check out the [issues](https://github.com/rbnpontes/DiligentEngineNET/issues) and feel free to submit pull requests.

---

## Acknowledgments

This project builds on the impressive work done by the [Diligent Graphics](https://github.com/DiligentGraphics/DiligentCore) team. Special thanks to their continued efforts in advancing cross-platform rendering technology.
