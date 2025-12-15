# Expandable First-Person Controller

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/your-username/your-repo-name)](https://github.com/your-username/your-repo-name/releases)

A modular and easily expandable first-person character controller for [Game Engine, e.g., Unity]. Designed to provide a solid foundation for player movement that can be extended with new abilities without modifying the core code.

![Showcase GIF](path/to/your/showcase.gif)
*A brief demonstration of the controller in action.*

---

## Table of Contents

- [Key Features](#key-features)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
- [How to Use](#how-to-use)
  - [Basic Setup](#basic-setup)
  - [Configuration](#configuration)
  - [Player Input](#player-input)
- [Architecture & Expansion](#architecture--expansion)
  - [Core Concepts](#core-concepts)
  - [Creating a New Ability](#creating-a-new-ability)
- [API Reference](#api-reference)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

---

## Key Features

*   **🚶‍♂️ Core Movement:** Smooth and responsive walk, run, and jump mechanics.
*   **👁️ First-Person Camera:** Includes mouse-look, configurable sensitivity, and pitch clamping.
*   **🧩 Modular by Design:** Easily add new abilities like crouching, sliding, wall-running, or grappling hooks as separate modules.
*   **⚙️ Highly Configurable:** Tweak movement variables, camera settings, and ability parameters directly from the Inspector.
*   **🕹️ Input System Agnostic:** Designed to work with any input system (e.g., Unity's Input Manager, New Input System, Rewired).
*   **✅ Clean & Organized Code:** Written with clarity and maintainability in mind, including detailed comments.

---

## Getting Started

Follow these steps to get the controller up and running in your project.

### Prerequisites

*   Unity 6.3 LTS
*   Input System

### Installation

1.  **Download:**
    *   **Option A (Recommended):** Go to the [Releases page](https://github.com/your-username/your-repo-name/releases) and download the latest `.unitypackage`.
    *   **Option B (From Git):** Clone this repository into your project's `Assets` folder.
        ```bash
        git clone https://github.com/your-username/your-repo-name.git YourProject/Assets/FirstPersonController
        ```
2.  **Import:**
    *   If you downloaded the `.unitypackage`, import it into your Unity project by navigating to `Assets > Import Package > Custom Package...`.

---

## How to Use

### Basic Setup

1.  Drag the `Player` and `Input Manager` prefab from `[Assets/PlayerBase/Prefab]` into your scene.
2.  Ensure your scene has only the camera included in the Player Prefab.
3.  Hit Play!

### Configuration

Select the `Player` object in your scene. In the Inspector, you will find several components that allow you to configure the controller's behavior:

*   **`PlayerMovement`:** Adjust variables like `walkSpeed`, `runSpeed`, `jumpForce`, and `gravity`.
*   **`PlayerLook`:** Change `mouseSensitivity` and the min/max look angles (`minXClamp`, `maxXClamp`).
*   **`[AbilityName]` (e.g., `PlayerCrouch`):** Configure parameters specific to each ability, such as `crouchHeight` or `slideSpeed`.

### Player Input

This controller listens for input through a scriptable object or an interface to remain input-system agnostic. To map your controls:

1.  Navigate to `[Assets/PlayerBase/Code/InputSystem/Controls]`.
2.  Modify the `InputData` Scriptable Object (or equivalent) to map your desired keys/buttons to actions like `Move`, `Jump`, `Run`, etc.
3.  *(If using Unity's New Input System)*: Ensure the `PlayerInput` component on the `Player` prefab is linked to your `Input Actions` asset.

---

## Architecture & Expansion

The controller is built around a central `PlayerController` script that manages a collection of "Abilities" (or "States"). Each ability is a self-contained component responsible for a single piece of functionality.

### Core Concepts

*   **`PlayerController.cs`:** The main orchestrator. It doesn't contain movement logic itself but instead delegates to the active ability. It manages the state transitions between abilities.
*   **`AbilityBase.cs`:** An abstract base class that all abilities must inherit from. It defines a common interface with methods like `OnEnter()`, `OnUpdate()`, and `OnExit()`.
*   **`Ability Components`:** (e.g., `WalkAbility.cs`, `JumpAbility.cs`). These are concrete implementations of `AbilityBase`. You can enable or disable them on the player prefab to control which abilities are available.

### Creating a New Ability

Adding a new movement mechanic is straightforward:

1.  **Create a new C# script** (e.g., `DashAbility.cs`) that inherits from `AbilityBase`.
2.  **Implement the abstract methods:**
    *   `CanEnter()`: Define the conditions under which this ability can become active (e.g., player is on the ground and presses the dash button).
    *   `OnEnter()`: Code to run once when the ability starts (e.g., apply an initial dash force).
    *   `OnUpdate()`: The main logic loop for the ability.
    *   `OnExit()`: Cleanup code to run when the ability ends.
3.  **Add the new script** as a component to your `Player` prefab.
4.  The `PlayerController` will automatically discover and manage the new ability.

---

## API Reference

Key public methods and properties for scripting interactions.

*   `PlayerController.IsGrounded`: Is the player currently on the ground?
*   `PlayerController.Move(Vector3 direction)`: Public method to apply external movement forces.
*   `PlayerController.EnableAbility<T>()` / `PlayerController.DisableAbility<T>()`: To enable or disable a specific ability at runtime.

*(Expand this section with more details about your public API.)*

---

## Contributing

Contributions are welcome! If you'd like to contribute, please fork the repository and create a pull request. You can also open an issue with the "enhancement" tag to suggest new features.

---

## License

This project is licensed under the MIT License - see the LICENSE.md file for details.
