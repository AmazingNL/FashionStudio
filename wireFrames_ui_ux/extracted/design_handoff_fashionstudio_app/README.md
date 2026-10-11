# Handoff: FashionStudio mobile app

## Overview
FashionStudio is a studio-management app for a solo tailor/fashion designer: today's fittings and due orders at a glance, a sketch gallery ("Studio"), a client book with measurements, a schedule, order tracking with payments, and quick capture flows for new sketches and orders.

## About the Design Files
The files in this bundle are **design references created in HTML** — prototypes showing intended look and behavior, not production code to copy directly. The task is to **recreate these designs in the target codebase's existing environment** (React Native, Flutter, SwiftUI, web, etc.) using its established patterns and libraries. If no environment exists yet, choose the most appropriate mobile framework and implement the designs there.

`FashionStudio Mockups.dc.html` is a review board showing all screens side-by-side in iPhone frames. Each screen's markup is inline-styled HTML inside the file — search for the screen ids below (`id="1a"` … `id="1j"`).

## Fidelity
**High-fidelity.** Colors, typography, spacing, radii and copy are final. Recreate pixel-perfectly. Screens 1a–1c are three alternative Home directions — **1a is the chosen direction**; 1b and 1c are explorations kept for reference. Screens 1d–1j are built in the 1a direction.

## Design system: "Organic"
All values come from the bundled `styles.css` token sheet. Key tokens:

Colors:
- `--color-bg: #f5ead8` (app background, cream)
- `--color-surface: #ebddc5` (sand, placeholder fills)
- `--color-text: #201e1d` (near-black)
- `--color-accent: #c67139` (terracotta — primary actions, active nav, FAB)
- `--color-accent-2: #7a8a5e` (sage — second voice: fitting statuses, notes cards, due banners)
- Each role has a 100–900 OKLCH ramp: `--color-neutral-*`, `--color-accent-*`, `--color-accent-2-*`. Light steps (100–300) = tinted fills/borders; 700–900 = text on tinted fills.
- `--color-divider` for card borders (1.5px) and row rules (1px).

Typography:
- Headings: Caprasimo (`--font-heading`) — screen titles 28px, section headings 18–26px, stat numbers 20–28px.
- Body: Figtree (`--font-body`) — list titles 15px/700, secondary 12.5px/`neutral-600`, section labels 11px/700/uppercase/1.6px letter-spacing.
- Both are Google Fonts.

Shape & elevation:
- Cards: `--radius-lg` (grows from 16px base), 1.5px `--color-divider` border, `--shadow-sm`.
- Buttons, inputs, chips, avatars: fully rounded (`border-radius: 999px`).
- Shadows: `--shadow-sm/md/lg` only.

Icons: Lucide (https://lucide.dev) at **stroke-width 2.75**, 15–26px.

Interaction states (from the system, apply everywhere):
- Hover/pressed: one ramp step past base (`--color-accent-600` pressed on light ground); tints for outlined variants.
- Focus: `outline: 2px solid var(--color-accent); outline-offset: 2px`.
- Disabled: 45% opacity.

## Navigation
Bottom tab bar on top-level screens: Home, Studio, [center FAB], Clients, Schedule.
- Bar: `neutral-100` bg, 1.5px divider top border. Active tab: `accent-700` + 700 weight; inactive: `neutral-600`.
- Center FAB: 54px terracotta circle, white plus, raised ~30px above the bar, `--shadow-md`. Opens a "New sketch / New order" choice (sheet suggested — not mocked).
- Detail screens (profile, order detail, new sketch/order) are pushed full-screen with a circular back/close button (36px, divider border) — no tab bar.

## Screens

### 1a — Home (chosen direction)
Purpose: today at a glance.
Layout, top to bottom (20px side padding, 12px column gap):
1. Header row: "FashionStudio" Caprasimo 26px; right, 42px avatar circle (`accent-200` bg / `accent-800` initials "AG").
2. Subline: "Tuesday, Aug 25 — two fittings today" 13.5px `neutral-600`.
3. "TODAY" section label; card with 3 rows separated by 1px dividers (14px 18px row padding): time (15px/800, 44px column) or a "DUE" pill (`accent-200`/`accent-800`, 10.5px/800), title 15px/700, sub 12.5px `neutral-600`, chevron-right 16px.
   - 10:00 Fitting — Adaeze O. / Order #041 · second fitting
   - 14:30 Fitting — Chidi N. / Order #043 · first fitting
   - DUE Order #039 — wrap dress / Ready for pickup by Thu
4. "ORDERS IN PROGRESS": 3-column grid of stat tiles (`radius-md`, 14px 8px padding) — 4 Sewing, 2 Fitting (neutral tiles), 1 Due soon (`accent-200` tile, `accent-800` text). Numbers Caprasimo 24px.
5. "RECENT SKETCHES" + "See all" link (13px/700 `accent-700`): 3-column grid of 3:4 thumbnails (`surface` fill, divider border, `radius-md`, centered Lucide scissors placeholder) with 11.5px captions (Look 07 — wrap, Look 08 — two-piece, Look 09 — gown).
6. Action row: "+ New sketch" solid terracotta pill (white 15px/700, 14px vertical padding, `shadow-sm`) and "+ New order" outlined pill (2px accent border, `accent-700` text).
7. Tab bar (Home active).

### 1b — Home alt (reference only): agenda-first
Top pill nav row instead of tab bar (active pill solid accent, others 1.5px divider outline). Caprasimo date heading 26px, count chips (4 sewing / 2 fitting / 1 due soon as tinted pills), vertical timeline (12px dots: filled accent = confirmed, outlined = upcoming, sage = due date; 2px divider connector), horizontal sketch strip, full-width "Create new order" pill. Formal copy.

### 1c — Home alt (reference only): bold Organic
33px Caprasimo greeting ("Good morning, Amara."), sage "Next up" hero card (`accent-2-200` bg, decorative `accent-2-300` circle bleeding off the corner, `accent-2-700` "Prep notes" pill), three 92px stat circles (accent-200 / accent-2-200 / solid accent; middle offset 18px down), overlapping 104px circular sketch thumbnails (-22px margin, 3px bg-colored borders). Playful copy. Tab bar as 1a.

### 1d — Studio (sketch gallery)
Title "Studio" 28px + circular search icon button. Filter chips: "All · 12" active (solid accent), Wraps / Two-piece / Gowns outlined. 2-column grid (10px gap) of 3:4 sketch cards: thumbnail (surface / `accent-2-100` / `accent-100` fills as placeholder variety), name 14px/700, then either a meta line (12px `neutral-600`) or an order pill (`accent-2-200`/`accent-2-800` "Order #043", `accent-200`/`accent-800` "Order #041"). Tab bar (Studio active).

### 1e — Clients (list)
Title "Clients" 28px + 40px terracotta circular add button. Search pill (`neutral-100`, divider border, 12px 18px padding, Lucide search 17px, placeholder "Search by name or order…" 14.5px `neutral-500`). One card containing 5 rows (13px 18px padding, 1px dividers): 44px initial avatar (alternating accent-200/accent-2-200/neutral-200 tints with matching 800-step text), name 15px/700, status line 12.5px `neutral-600`, chevron. Footer: "18 clients · sorted by activity" centered 12.5px. Tab bar (Clients active).

### 1f — Client profile
Back button + "Client" label. 64px avatar + name Caprasimo 24px + "Client since March 2024 · 6 orders". Action row: outlined "Call" pill + solid "Message" pill (both with 15px Lucide icons). Measurements card: label row + "Updated 12 Aug", 4-column grid of value (Caprasimo 20px) over label (11.5px `neutral-600`) — Bust 92, Waist 74, Hips 98, Length 132 — footnote "cm · full sheet has 14 measurements". Orders card: rows with title 14.5px/700, sub, status pill ("In fitting" sage tint, "Delivered" neutral tint). Notes card: `accent-2-100` bg, `accent-2-200` border, `accent-2-900` body 13.5px/1.5.

### 1g — Schedule
Title "Schedule" 28px + "August 2026" 13px/700 `accent-700`. Week strip Mon–Sat: 40px day circles — today (TUE 25) solid accent white text, day with deadline (THU 27) 2px sage outline, others plain. Sections per day (label + card):
- Tuesday 25 — today: 10:00 / 14:30 fitting rows with 10px accent dot on the right.
- Thursday 27: sage card (`accent-2-100` bg) with white-on-`accent-2-700` "DUE" pill — "Order #039 — pickup, Bisi A." / "Wrap dress · final press Wednesday".
- Friday 28: empty state — 1.5px dashed `neutral-400` border card, "No appointments — good sewing day" + "+ Add" link.
Tab bar (Schedule active).

### 1h — Order detail
Back button + "Order #041". Title "Wrap dress — Look 07" Caprasimo 24px + "Adaeze Okafor · due Thu, 4 Sep"; status pill "In fitting" (sage tint) top-right.
- Status timeline card: 4 steps (Cut ✓ 8 Aug, Sewing ✓ 15 Aug, Fitting ● today, Ready ○ 4 Sep). Done: 26px `accent-2-500` circles with white check; current: solid accent circle with 4px `accent-200` halo, label `accent-700`/800; upcoming: 2.5px `neutral-400` outline. Connectors 2.5px (sage done / neutral-300 upcoming).
- Fabric & materials card: rows of 32px color-swatch circle + name 14px/700 + meta 12px (Ankara — "Sunset" 2.5 yd · from client; Cotton lining 1 yd · studio stock; Covered buttons × 6 to order — reminder set).
- Payment card: "Record payment" link right; "₦50,000" Caprasimo 22px + "of ₦85,000 paid"; 10px progress pill (neutral-200 track, 59% accent fill); "Balance ₦35,000 · due at pickup".
- Notes card: sage tint (as 1f).

### 1i — New sketch (capture-first)
Header: close ✕ (left), "New sketch" Caprasimo 19px, "Save" disabled (`neutral-400`). Capture zone: 2px dashed `accent-400` border, `accent-100` fill, `radius-lg`, 44px 24px padding — 64px terracotta camera circle, "Photograph your sketch" 16px/700 `accent-800`, "or choose from your library" 13px `accent-700`. Fields (label 12.5px/700): Title — pill input with placeholder "Look 10 — give it a name…"; Garment type — chip row (Wrap active solid, Gown/Two-piece/Suit outlined, "+" dashed); Notes (optional) — `radius-md` textarea, min-height 56px. Footer: solid "Save sketch" pill.

### 1j — New order
Header: close ✕, "New order" 19px. Fields:
- Client: pill selector with 34px avatar + "Adaeze Okafor" + chevron-down.
- Garment: segmented control — `neutral-200` track pill, 4px padding; selected segment `neutral-100` fill + `shadow-sm`; Dress | Top | Skirt | Suit.
- From sketch: sage card (`accent-2-100`/`accent-2-200`) with 42×54px thumbnail, "Look 07 — wrap" + "Tap to change or skip", chevron.
- Due date / Price: two half-width pill inputs ("Thu, 4 Sep" / "₦85,000", 14px/700).
- Deposit received: pill with "₦50,000" left, "59% of total" right.
Footer: solid "Create order #044" pill + helper "First fitting will be suggested for next week" centered 12.5px.

## Interactions & Behavior
- Tab bar switches top-level screens; FAB opens new-sketch/new-order choice.
- Home: today rows → order detail (1h); "See all" → Studio (1d); action pills → 1i / 1j.
- Studio: filter chips filter the grid; card → sketch detail (not mocked — suggest order-detail-style layout); search expands the search field.
- Clients: row → profile (1f); add button → new-client form (not mocked); search filters live.
- Profile: Call/Message open tel:/messaging; order rows → 1h; measurements card → full measurement sheet (not mocked).
- Schedule: week strip selects the day and scrolls the agenda; "+ Add" → new appointment.
- Order detail: "Record payment" opens amount entry; advancing status moves the timeline (animate the connector fill, ~300ms ease-out).
- New sketch: Save disabled until a photo is captured; camera zone opens camera with library fallback.
- New order: order number auto-increments; creating schedules a suggested first fitting.
- Transitions: push (slide-in) for detail screens, modal sheet for 1i/1j. Pressed states per design system (one ramp step darker).

## State Management
- Entities: Client (id, name, initials, contact, measurements[14], notes, createdAt), Sketch (id, title, garmentType, photo, notes, orderId?), Order (id, number, clientId, sketchId?, garment, status: cut|sewing|fitting|ready|delivered, statusDates, dueDate, price, payments[], materials[], notes), Appointment (id, orderId, clientId, type, start, duration).
- Home derives: today's appointments, due-soon orders (due ≤ 3 days), status counts, 3 latest sketches.
- Currency: Nigerian naira (₦), formatted with thousands separators.

## Assets
No raster assets. Sketch thumbnails are placeholders (Lucide `scissors` on surface fills) — replace with user photo uploads. Avatars are initials on tinted circles. Fonts: Caprasimo + Figtree (Google Fonts). Icons used: scissors, home, users, calendar, plus, chevron-right/left/down, x, search, check, camera, phone, message-circle.

## Files
- `FashionStudio Mockups.dc.html` — the review board with all 10 screens (design reference; screen markup inline, find by `id="1a"`…`id="1j"`).
- `styles.css` — the Organic design-system token sheet and component classes (source of every color/font/spacing/radius/shadow value).
- `ios-frame.jsx` — iPhone frame used for presentation only; not part of the design.
