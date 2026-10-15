import React from 'react';
import { StyleSheet, Text, TextInput, TextInputProps, View } from 'react-native';
import { colors, fontFamily, fontSize, radius, spacing } from '../theme';

interface TextFieldProps extends TextInputProps {
  label: string;
}

// Mirrors styles.css .field/.input: labeled pill-ish input.
export function TextField({ label, style, ...rest }: TextFieldProps) {
  return (
    <View style={styles.field}>
      <Text style={styles.label}>{label}</Text>
      <TextInput
        style={[styles.input, style]}
        placeholderTextColor={colors.neutral500}
        selectionColor={colors.accent}
        {...rest}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  field: { gap: 5 },
  label: {
    fontFamily: fontFamily.bodySemiBold,
    fontSize: fontSize.label,
    color: colors.neutral700,
  },
  input: {
    minHeight: 46,
    paddingHorizontal: spacing[3],
      borderRadius: radius.lg,
    borderWidth: 1,
    borderColor: colors.divider,
    backgroundColor: colors.surface,
    fontFamily: fontFamily.bodyRegular,
    fontSize: fontSize.body,
    color: colors.text,
  },
});
