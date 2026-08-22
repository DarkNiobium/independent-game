# Minimalist Main Menu Specification

## Overview
A clean, cinematic, and minimalist Main Menu for "Mustaqillik Yo'li" (Way to Independence). It removes all clutter, side widgets, and complex card systems, focusing purely on atmosphere, refined typography, and 4 core navigation actions.

## Key Features
- **Atmospheric Background:** Wide cinematic silhouette of an ancient Silk Road skyline at dusk with warm gradient skies.
- **Refined Typography:** Centered titles with Cinzel serif font in warm sand and gold palettes.
- **Minimalist Menu Stack:**
  1. **Boshlash** (Start Game) -> Loads the city-building scene.
  2. **Davom etish** (Continue Game) -> Resumes progress.
  3. **Sozlamalar** (Settings) -> Opens the oriental Settings Menu overlay without reloading scenes.
  4. **Chiqish** (Exit) -> Clean application quit.
- **Micro-Interactions:** Subtle scaling and golden luminescence on button hover.
- **Embedded Settings Overlay:** Uses our Settings UI as a modal overlay within the Main Menu for instant responsiveness.
- **Automated Scene Builder:** `MainMenuSceneBuilder.cs` creates `Assets/Scenes/MainMenuScene.unity` and sets it at index 0 in `EditorBuildSettings`.
