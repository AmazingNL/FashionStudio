import React, { useState } from 'react';
import { Pressable, StyleSheet, Text, View } from 'react-native';
import { useNavigation, useRoute } from '@react-navigation/native';
import { Button, Screen, TextField } from '../components';
import { ApiError, paymentsApi } from '../api';
import type { PaymentMethod } from '../api';
import { colors, fontFamily, fontSize, radius, spacing } from '../theme';

const METHODS: PaymentMethod[] = ['Cash', 'CreditCard', 'MobilePayment', 'BankTransfer', 'Other'];
const METHOD_LABELS: Record<PaymentMethod, string> = {
  Cash: 'Cash',
  CreditCard: 'Card',
  MobilePayment: 'Mobile',
  BankTransfer: 'Transfer',
  Other: 'Other',
};

export function RecordPaymentScreen() {
  const navigation = useNavigation<any>();
  const route = useRoute<any>();
  const { orderId, balance } = route.params as { orderId: number; balance: number };

  const [amount, setAmount] = useState(balance > 0 ? String(balance) : '');
  const [method, setMethod] = useState<PaymentMethod>('Cash');
  const [notes, setNotes] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  async function handleSave() {
    const parsedAmount = Number(amount);
    if (!parsedAmount || parsedAmount <= 0) {
      setError('Enter an amount greater than zero.');
      return;
    }
    setError(null);
    setSaving(true);
    try {
      await paymentsApi.createPayment({
        orderId,
        amount: parsedAmount,
        method,
        paymentDate: new Date().toISOString(),
        notes: notes.trim(),
      });
      navigation.goBack();
    } catch (err) {
      // Most likely cause: PaymentService's balance-due cap rejected an
      // amount that overshoots what's left on the order.
      setError(err instanceof ApiError ? err.message : 'Could not record this payment.');
    } finally {
      setSaving(false);
    }
  }

  return (
    <Screen>
      <Text style={styles.title}>Record payment</Text>
      <Text style={styles.subtitle}>Balance remaining: {balance.toLocaleString()}</Text>

      <TextField label="Amount" value={amount} onChangeText={setAmount} keyboardType="numeric" />

      <View style={styles.field}>
        <Text style={styles.label}>Method</Text>
        <View style={styles.segmented}>
          {METHODS.map((m) => (
            <Pressable
              key={m}
              style={[styles.segment, method === m && styles.segmentActive]}
              onPress={() => setMethod(m)}
            >
              <Text style={[styles.segmentLabel, method === m && styles.segmentLabelActive]}>
                {METHOD_LABELS[m]}
              </Text>
            </Pressable>
          ))}
        </View>
      </View>

      <TextField label="Notes (optional)" value={notes} onChangeText={setNotes} />

      {error && <Text style={styles.error}>{error}</Text>}

      <Button label="Save payment" onPress={handleSave} loading={saving} fullWidth />
    </Screen>
  );
}

const styles = StyleSheet.create({
  title: { fontFamily: fontFamily.heading, fontSize: fontSize.screenTitle, color: colors.text, marginTop: spacing[3] },
  subtitle: { fontFamily: fontFamily.bodyRegular, fontSize: fontSize.secondary, color: colors.neutral600, marginBottom: spacing[2] },
  field: { gap: 5 },
  label: { fontFamily: fontFamily.bodySemiBold, fontSize: fontSize.label + 1, color: colors.neutral700 },
  segmented: {
    flexDirection: 'row',
    borderWidth: 1,
    borderColor: colors.divider,
    borderRadius: radius.md,
    overflow: 'hidden',
  },
  segment: { flex: 1, paddingVertical: 10, alignItems: 'center' },
  segmentActive: { backgroundColor: colors.accent },
  segmentLabel: { fontFamily: fontFamily.bodySemiBold, fontSize: 12, color: colors.text },
  segmentLabelActive: { color: colors.bg },
  error: { fontFamily: fontFamily.bodySemiBold, fontSize: fontSize.secondary, color: colors.accent700 },
});
