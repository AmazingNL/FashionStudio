import React from 'react';
import { ActivityIndicator, GestureResponderEvent, Pressable, StyleSheet, Text } from 'react-native';
import { colors, fontFamily, radius, shadow, spacing } from '../theme';

export type ButtonVariant = 'primary' | 'secondary';

interface ButtonProps {
  label: string;
  onPress?: (event: GestureResponderEvent) => void;
  variant?: ButtonVariant;
  disabled?: boolean;
  loading?: boolean;
  fullWidth?: boolean;
}

// Mirrors styles.css .btn / .btn-primary / .btn-secondary / .btn-block.
export function Button({ label, onPress, variant = 'primary', disabled, loading, fullWidth }: ButtonProps) {
  const isDisabled = disabled || loading;
  return (
    <Pressable
      onPress={onPress}
      disabled={isDisabled}
      style={({ pressed }) => [
        styles.base,
        variant === 'primary' ? styles.primary : styles.secondary,
        fullWidth && styles.fullWidth,
        isDisabled && styles.disabled,
        pressed && !isDisabled && (variant === 'primary' ? styles.primaryPressed : styles.secondaryPressed),
      ]}
    >
      {loading ? (
        <ActivityIndicator color={variant === 'primary' ? colors.bg : colors.text} />
      ) : (
        <Text style={[styles.label, variant === 'primary' ? styles.primaryLabel : styles.secondaryLabel]}>
          {label}
        </Text>
      )}
    </Pressable>
  );
}

const styles = StyleSheet.create({
  base: {
    alignItems: 'center',
    justifyContent: 'center',
    borderRadius: radius.pill,
    paddingVertical: spacing[3],
    paddingHorizontal: spacing[4],
  },
  fullWidth: { width: '100%' },
  primary: { backgroundColor: colors.accent, ...shadow.sm },
  primaryPressed: { backgroundColor: colors.accent700 },
  secondary: { borderWidth: 2, borderColor: colors.accent },
  secondaryPressed: { backgroundColor: colors.accent100 },
  disabled: { opacity: 0.45 },
  label: { fontFamily: fontFamily.bodyBold, fontSize: 15 },
  primaryLabel: { color: colors.bg },
  secondaryLabel: { color: colors.accent700 },
});
