import React, { useState } from 'react';
import { StyleSheet, Text } from 'react-native';
import { useNavigation } from '@react-navigation/native';
import { Button, Screen, TextField } from '../components';
import { ApiError, customersApi } from '../api';
import { colors, fontFamily, fontSize, spacing } from '../theme';

// Not part of the mocked screens (README: "add-button -> new-client form
// (not mocked)") — built plainly with the same design tokens/components
// rather than inventing a pixel-perfect layout nothing asked for.
export function NewClientScreen() {
  const navigation = useNavigation<any>();
  const [fullName, setFullName] = useState('');
  const [phone, setPhone] = useState('');
  const [email, setEmail] = useState('');
  const [notes, setNotes] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  async function handleSave() {
    if (!fullName.trim() || !phone.trim()) return;
    setError(null);
    setSaving(true);
    try {
      const customer = await customersApi.createCustomer({
        fullName: fullName.trim(),
        phone: phone.trim(),
        email: email.trim(),
        notes: notes.trim(),
      });
      navigation.replace('ClientProfile', { customerId: customer.id });
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Could not save this client.');
    } finally {
      setSaving(false);
    }
  }

  return (
    <Screen>
      <Text style={styles.title}>New client</Text>
      <TextField label="Full name" value={fullName} onChangeText={setFullName} autoCapitalize="words" />
      <TextField label="Phone" value={phone} onChangeText={setPhone} keyboardType="phone-pad" />
      <TextField label="Email (optional)" value={email} onChangeText={setEmail} autoCapitalize="none" keyboardType="email-address" />
      <TextField label="Notes (optional)" value={notes} onChangeText={setNotes} multiline style={{ minHeight: 80, paddingTop: 10 }} />
      {error && <Text style={styles.error}>{error}</Text>}
      <Button label="Save client" onPress={handleSave} loading={saving} disabled={!fullName.trim() || !phone.trim()} fullWidth />
    </Screen>
  );
}

const styles = StyleSheet.create({
  title: { fontFamily: fontFamily.heading, fontSize: fontSize.screenTitle, color: colors.text, marginTop: spacing[3] },
  error: { fontFamily: fontFamily.bodySemiBold, fontSize: fontSize.secondary, color: colors.accent700 },
});
