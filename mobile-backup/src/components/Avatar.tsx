import React from 'react';
import { StyleSheet, Text, View } from 'react-native';
import { colors, fontFamily } from '../theme';

interface AvatarProps {
  initials: string;
  size?: number;
  tone?: 'accent' | 'accent2' | 'neutral';
}

const toneBg: Record<NonNullable<AvatarProps['tone']>, string> = {
  accent: colors.accent200,
  accent2: colors.accent2_200,
  neutral: colors.neutral200,
};
const toneText: Record<NonNullable<AvatarProps['tone']>, string> = {
  accent: colors.accent800,
  accent2: colors.accent2_800,
  neutral: colors.neutral800,
};

export function Avatar({ initials, size = 42, tone = 'accent' }: AvatarProps) {
  return (
    <View
      style={[
        styles.circle,
        { width: size, height: size, borderRadius: size / 2, backgroundColor: toneBg[tone] },
      ]}
    >
      <Text style={[styles.initials, { color: toneText[tone], fontSize: size * 0.38 }]}>{initials}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  circle: { alignItems: 'center', justifyContent: 'center' },
  initials: { fontFamily: fontFamily.bodyBold },
});
