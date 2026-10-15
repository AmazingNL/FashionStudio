import React, { useState } from 'react';
import { StyleSheet, Text } from 'react-native';
import { Button, Screen, TextField } from '../components';
import { useAuth } from '../context/AuthContext';
import { ApiError } from '../api';
import { colors, fontFamily, fontSize, spacing } from '../theme';

// Shown when the signed-in user has no workspace yet — a brand new account,
// or an existing one that was never invited anywhere. Creating one here
// makes them its Owner (see WorkSpaceService.CreateWorkSpaceAsync).
export function NeedsWorkSpaceScreen() {
  const { createFirstWorkSpace } = useAuth();
  const [name, setName] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function handleCreate() {
    if (!name.trim()) return;
    setError(null);
    setLoading(true);
    try {
      await createFirstWorkSpace(name.trim());
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Something went wrong. Please try again.');
    } finally {
      setLoading(false);
    }
  }

  return (
    <Screen>
      <Text style={styles.title}>Name your studio</Text>
      <Text style={styles.subtitle}>
        This is the workspace your clients, orders, and fittings will live in. You can invite assistants and
        tailors to it later.
      </Text>
      <TextField label="Studio name" value={name} onChangeText={setName} placeholder="e.g. Grace Couture" />
      {error && <Text style={styles.error}>{error}</Text>}
      <Button label="Create studio" onPress={handleCreate} loading={loading} disabled={!name.trim()} fullWidth />
    </Screen>
  );
}

const styles = StyleSheet.create({
  title: { fontFamily: fontFamily.heading, fontSize: fontSize.screenTitle, color: colors.text, marginTop: spacing[8] },
  subtitle: { fontFamily: fontFamily.bodyRegular, fontSize: fontSize.body, color: colors.neutral700, marginBottom: spacing[2] },
  error: { fontFamily: fontFamily.bodySemiBold, fontSize: fontSize.secondary, color: colors.accent700 },
});
