export const spacing = {
  1: 4.4,
  2: 8.8,
  3: 13.2,
  4: 17.6,
  6: 26.4,
  8: 35.2,
} as const;

export const radius = {
  sm: 8,
  md: 16,
  lg: 28,
  pill: 999,
} as const;

// React Native has no direct box-shadow — iOS uses shadow*/ Android uses
// elevation. These three presets mirror styles.css's --shadow-sm/md/lg.
export const shadow = {
  sm: {
    shadowColor: '#2e2b25',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.14,
    shadowRadius: 2,
    elevation: 2,
  },
  md: {
    shadowColor: '#2e2b25',
    shadowOffset: { width: 0, height: 3 },
    shadowOpacity: 0.16,
    shadowRadius: 10,
    elevation: 4,
  },
  lg: {
    shadowColor: '#2e2b25',
    shadowOffset: { width: 0, height: 12 },
    shadowOpacity: 0.22,
    shadowRadius: 32,
    elevation: 8,
  },
} as const;
