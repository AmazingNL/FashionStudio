import React from 'react';
import { ScrollView, ScrollViewProps, StyleSheet, View, ViewProps } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { colors, spacing } from '../theme';

interface ScreenProps extends ViewProps {
  scroll?: boolean;
  refreshControl?: ScrollViewProps['refreshControl'];
}

// Consistent cream background + side padding + safe-area handling for every
// screen, so individual screens don't repeat this boilerplate.
export function Screen({ scroll = true, refreshControl, style, children, ...rest }: ScreenProps) {
  if (!scroll) {
    return (
      <SafeAreaView style={styles.safeArea} edges={['top', 'bottom']}>
        <View style={[styles.content, style]} {...rest}>
          {children}
        </View>
      </SafeAreaView>
    );
  }
  return (
    <SafeAreaView style={styles.safeArea} edges={['top', 'bottom']}>
      <ScrollView contentContainerStyle={[styles.content, style]} refreshControl={refreshControl} {...rest}>
        {children}
      </ScrollView>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safeArea: { flex: 1, backgroundColor: colors.bg },
  content: { paddingHorizontal: spacing[4], paddingBottom: spacing[8], gap: spacing[3] },
});
