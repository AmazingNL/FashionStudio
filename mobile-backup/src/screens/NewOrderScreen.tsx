import React, { useEffect, useState } from 'react';
import { Pressable, StyleSheet, Text, View } from 'react-native';
import { useNavigation, useRoute } from '@react-navigation/native';
import { ChevronDown, X } from 'lucide-react-native';
import { Button, Screen, TextField } from '../components';
import { ApiError, ordersApi } from '../api';
import { useAuth } from '../context/AuthContext';
import { colors, fontFamily, fontSize, radius, spacing } from '../theme';

export function NewOrderScreen() {
  const navigation = useNavigation<any>();
  const route = useRoute<any>();
  const { workSpace } = useAuth();

  const [customerId, setCustomerId] = useState<number | null>(null);
  const [customerName, setCustomerName] = useState('');
  const [title, setTitle] = useState('');
  const [dueDate, setDueDate] = useState('');
  const [price, setPrice] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  // Picked up from SelectClientScreen navigating back with { merge: true }.
  useEffect(() => {
    if (route.params?.selectedCustomerId) {
      setCustomerId(route.params.selectedCustomerId);
      setCustomerName(route.params.selectedCustomerName);
    }
  }, [route.params?.selectedCustomerId, route.params?.selectedCustomerName]);

  async function handleCreate() {
    if (!workSpace || !customerId || !title.trim() || !dueDate.trim() || !price) return;
    const parsedPrice = Number(price);
    if (!parsedPrice || parsedPrice <= 0) {
      setError('Enter a price greater than zero.');
      return;
    }
    const parsedDate = new Date(dueDate);
    if (Number.isNaN(parsedDate.getTime())) {
      setError('Enter the due date as YYYY-MM-DD.');
      return;
    }
    setError(null);
    setSaving(true);
    try {
      // The API requires both a deadlineDate and an eventDate — the mock
      // only has one "due date" field, so both are set to the same value
      // here rather than inventing a second date input nothing asked for.
      const isoDate = parsedDate.toISOString();
      const order = await ordersApi.createOrder({
        customerId,
        workSpaceId: workSpace.id,
        title: title.trim(),
        quotedPrice: parsedPrice,
        deadlineDate: isoDate,
        eventDate: isoDate,
      });
      navigation.replace('OrderDetail', { orderId: order.id });
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Could not create this order.');
    } finally {
      setSaving(false);
    }
  }

  const canSubmit = !!customerId && !!title.trim() && !!dueDate.trim() && !!price;

  return (
    <Screen>
      <View style={styles.header}>
        <Pressable onPress={() => navigation.goBack()}>
          <X color={colors.text} size={20} strokeWidth={2.75} />
        </Pressable>
        <Text style={styles.title}>New order</Text>
        <View style={{ width: 20 }} />
      </View>

      <View style={styles.field}>
        <Text style={styles.label}>Client</Text>
        <Pressable style={styles.clientPicker} onPress={() => navigation.navigate('SelectClient')}>
          <Text style={customerName ? styles.clientPickerText : styles.clientPickerPlaceholder}>
            {customerName || 'Choose a client…'}
          </Text>
          <ChevronDown color={colors.neutral600} size={16} strokeWidth={2.75} />
        </Pressable>
      </View>

      <TextField label="Title" value={title} onChangeText={setTitle} placeholder="e.g. Wrap dress — Look 07" />
      <TextField label="Due date" value={dueDate} onChangeText={setDueDate} placeholder="YYYY-MM-DD" />
      <TextField label="Price" value={price} onChangeText={setPrice} keyboardType="numeric" placeholder="0" />

      {error && <Text style={styles.error}>{error}</Text>}

      <Button label="Create order" onPress={handleCreate} loading={saving} disabled={!canSubmit} fullWidth />
    </Screen>
  );
}

const styles = StyleSheet.create({
  header: { flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between', marginTop: spacing[2] },
  title: { fontFamily: fontFamily.heading, fontSize: 19, color: colors.text },
  field: { gap: 5 },
  label: { fontFamily: fontFamily.bodySemiBold, fontSize: fontSize.label + 1, color: colors.neutral700 },
  clientPicker: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    minHeight: 46,
    paddingHorizontal: spacing[3],
    borderRadius: radius.md,
    borderWidth: 1,
    borderColor: colors.divider,
    backgroundColor: colors.surface,
  },
  clientPickerText: { fontFamily: fontFamily.bodyRegular, fontSize: fontSize.body, color: colors.text },
  clientPickerPlaceholder: { fontFamily: fontFamily.bodyRegular, fontSize: fontSize.body, color: colors.neutral500 },
  error: { fontFamily: fontFamily.bodySemiBold, fontSize: fontSize.secondary, color: colors.accent700 },
});
