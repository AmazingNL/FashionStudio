// Font families are registered under these exact names in App.tsx via
// useFonts() — must match or text silently falls back to the system font.
export const fontFamily = {
  heading: 'Caprasimo_400Regular',
  bodyRegular: 'Figtree_400Regular',
  bodySemiBold: 'Figtree_600SemiBold',
  bodyBold: 'Figtree_700Bold',
} as const;

// Named sizes pulled from the README's documented values, not invented —
// screens should reach for these rather than picking arbitrary numbers.
export const fontSize = {
  screenTitle: 28,
  sectionHeadingLg: 26,
  sectionHeadingSm: 18,
  statNumber: 24,
  cardTitle: 17,
  listTitle: 15,
  body: 14.5,
  secondary: 12.5,
  label: 11,
  caption: 11.5,
} as const;
