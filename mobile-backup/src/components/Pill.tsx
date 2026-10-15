import React from 'react';
import { StyleSheet, Text, View } from 'react-native';
import { colors, fontFamily, fontSize } from '../theme';

export type PillTone = 'accent' | 'accent2' | 'neutral' | 'outline';

interface PillProps {
  label: string;
  tone?: PillTone;
}

// Mirrors styles.css .tag-*: small rounded status/label chip.
export function Pill({ label, tone = 'neutral' }: PillProps) {
  const toneStyle = toneStyles[tone];
  return (
    <View style={[styles.pill, toneStyle.container]}>
      <Text style={[styles.label, toneStyle.text]}>{label}</Text>
    </View>
  );
}

const toneStyles: Record<PillTone, { container: object; text: object }> = {
  accent: { container: { backgroundColor: colors.accent100 }, text: { color: colors.accent800 } },
  accent2: { container: { backgroundColor: colors.accent2_100 }, text: { color: colors.accent2_800 } },
  neutral: { container: { backgroundColor: colors.neutral100 }, text: { color: colors.neutral800 } },
  outline: { container: { borderWidth: 1, borderColor: colors.accent }, text: { color: colors.accent } },
};

const styles = StyleSheet.create({
  pill: {
    alignSelf: 'flex-start',
    paddingVertical: 3,
    paddingHorizontal: 10,
    borderRadius: 12,
  },
  label: {
    fontFamily: fontFamily.bodyBold,
    fontSize: fontSize.label,
    letterSpacing: 0.2,
  },
});
