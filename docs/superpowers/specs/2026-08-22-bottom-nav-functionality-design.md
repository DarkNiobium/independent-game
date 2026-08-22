# Bottom Navigation Bar Functional Architecture Design

## Overview
This design implements fully interactive bottom navigation tabs (`RESURSLAR`, `BINOLAR`, `ARMIYA`, `TADQIQOT`, `BOSHQA`) matching the authentic oriental visual style from the reference UI, with dynamic active state styling, section switching, and modular event callbacks.

## Components & Architecture

### 1. Sprites & Visuals
- `nav_bar_frame_clean.png`: Clean carved wood bottom bar without baked-in active highlight.
- `nav_active_pill.png`: Turquoise and gold ornate highlighted frame for the active tab.
- Individual crisp icons with transparency:
  - `icon_nav_resources.png`: Coin pouch
  - `icon_nav_buildings.png`: Oriental castle / fortress
  - `icon_nav_army.png`: Crossed scimitars / swords
  - `icon_nav_research.png`: Scroll and compass
  - `icon_nav_other.png`: Shield badge

### 2. C# Architecture
- `BottomNavSection`: Enum (`Resources`, `Buildings`, `Army`, `Research`, `Other`).
- `BottomNavItemUI`: Component on each tab button handling:
  - Button click callback
  - Active / Inactive sprite toggle (`nav_active_pill` on/off)
  - Text color & typography adjustments
- `BottomNavUI`: Central controller for the bottom navigation bar:
  - Manages active tab state
  - Fires `onSectionChanged` event (`Action<BottomNavSection>` and `UnityEvent<BottomNavSection>`)
  - Coordinates panel visibility across different game views (`BozorWindow` for `Buildings`, and themed panels for other sections)

### 3. Integration with Bozor Shop
- `BozorShopController` listens to `BottomNavUI` section changes.
- Selecting `BINOLAR` activates the Bozor market window.
- Selecting another section switches to that section's view cleanly.
