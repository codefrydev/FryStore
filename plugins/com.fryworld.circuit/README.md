# FryCircuit Studio Plugin for FryWorld

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/)
[![Avalonia UI 12](https://img.shields.io/badge/Avalonia-12.1.2-red.svg)](https://avaloniaui.net/)

An interactive SPICE-grade analog & digital schematic CAD studio and real-time MNA circuit simulator plugin for **FryWorld** and **FryPDF**.

---

## ⚡ Highlights

- **Interactive Schematic Canvas**: 60 FPS real-time transient solver with Modified Nodal Analysis (MNA), animated electron current flow particles, and live node voltage badges.
- **Rich Component Catalog**: Resistors, Capacitors, Inductors, AC/DC Voltage & Current Sources, Ground, SPST Switches, Potentiometers, Diodes, Zener Diodes, LEDs, BJTs (NPN/PNP), MOSFETs (NMOS/PMOS), Op-Amps, 555 Timers, Digital Logic Gates (AND, OR, NOT, NAND, NOR, XOR), 7-Segment Displays, and Quantum Josephson Junctions.
- **Dual-Channel Oscilloscope**: Real-time voltage & current probes with time-domain waveforms and live FFT Spectrum Analysis.
- **Industry Standards**: Berkeley SPICE 3f5 / LTspice / WRSPICE netlist deck import and export, plus aerospace-grade manufacturing Bill of Materials (BOM).
- **FryWorld Shell Integration**:
  - Full-viewport navigation page in Left Navigation Rail
  - Draggable picture-in-picture floating overlay
  - Command Palette shortcuts (`Ctrl+Alt+C` / `Cmd+Alt+C`)
  - Footer status bar widget (`⚡ Circuit`)
  - Dynamic Ribbon tab actions
  - Declarative Material Design 3 settings schema
- **Standalone Runner**: Run independently via `dotnet run --project Runner/CircuitPlugin.Runner.csproj` without requiring the host application.
