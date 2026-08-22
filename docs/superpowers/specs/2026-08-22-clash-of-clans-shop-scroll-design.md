# Clash of Clans Style Synchronized Horizontal Shop Design

## Overview
Transform the Bozor shop UI into a classic Clash of Clans horizontal continuous scroll marketplace. All buildings from all categories are displayed in a single unified horizontal `ScrollRect`, with a single synchronized top tab bar (`ISHLAB CHIQARISH`, `QISHLOQ XO'JALIGI`, `SAVDO`, `BEZAKLAR`).

## Key Requirements & Mechanics

### 1. Unified Horizontal Scroll View
- A single horizontal `ScrollRect` containing all building cards across all categories in consecutive order.
- Smooth drag, flick inertia, and scroll wheel support.
- Masked Viewport matching the internal parchment bounds.

### 2. Single Top Tab Bar with Synchronous Two-Way Binding
- Exactly **one row of tabs** located at the top above the cards:
  1. `ISHLAB CHIQARISH` (Production)
  2. `QISHLOQ XO'JALIGI` (Agriculture)
  3. `SAVDO` (Trade)
  4. `BEZAKLAR` (Decorations)
- **Content Scroll → Tab Update**: As the user scrolls horizontally through the building cards, the system detects which category is currently in the viewport and updates the active tab state in real-time.
- **Tab Click → Content Smooth Scroll**: Clicking a category tab starts a smooth scroll animation (Coroutine / SmoothDamp) directly to that category's starting position in the card list.

### 3. Clean Single-Row Framing
- Remove the bottom navigation bar to streamline the UI to a single coherent tab system, giving the building cards optimal framing and visual appeal.

## Components & Scripts

### `BozorShopController.cs`
- Manages the `ScrollRect`, category card clusters, and category start offsets.
- Handles smooth scroll coroutines on tab clicks.
- Tracks `scrollRect.horizontalNormalizedPosition` during manual drags to highlight the corresponding top tab.

### `ShopTabUI.cs`
- Retains authentic 9-slice active/inactive background styling and TextMeshPro typography.
- Triggers category selection event when clicked.

### `BozorShopBuilder.cs`
- Rebuilds `BozorShopScene.unity` with the `ScrollRect` viewport, unified cards container, single top tab bar, and removes redundant bottom bar elements.
