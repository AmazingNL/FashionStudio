import React, { useCallback, useState } from 'react';
import { ActivityIndicator, Linking, Pressable, StyleSheet, Text, View } from 'react-native';
import { useFocusEffect, useNavigation, useRoute } from '@react-navigation/native';
import { ArrowLeft, MessageCircle, Phone } from 'lucide-react-native';
import { Avatar, Button, Card, Pill, Screen } from '../components';
import type { PillTone } from '../components';
import { customersApi, ordersApi } from '../api';
import type { CustomerResponseDTO, OrderResponseDTO, OrderStatus } from '../api';
import { colors, fontFamily, fontSize, spacing } from '../theme';

function initialsFor(name: string) {
  const parts = name.trim().split(/\s+/);
  return ((parts[0]?.[0] ?? '') + (parts[1]?.[0] ?? '')).toUpperCase() || '?';
}

const statusTone: Record<OrderStatus, PillTone> = {
  New: 'neutral',
  InProgress: 'neutral',
  Fitting: 'accent2',
  Ready: 'accent',
  Delivered: 'neutral',
  Cancelled: 'neutral',
  Rejected: 'neutral',
};

export function ClientProfileScreen() {
  const navigation = useNavigation<any>();
  const route = useRoute<any>();
  const customerId: number = route.params.customerId;

  const [loading, setLoading] = useState(true);
  const [customer, setCustomer] = useState<CustomerResponseDTO | null>(null);
  const [orders, setOrders] = useState<OrderResponseDTO[]>([]);

  useFocusEffect(
    useCallback(() => {
      let cancelled = false;
      (async () => {
        setLoading(true);
        try {
          const [customerResult, ordersPage] = await Promise.all([
            customersApi.getCustomerById(customerId),
            ordersApi.getOrders({ pageSize: 100 }),
          ]);
          if (cancelled) return;
          setCustomer(customerResult);
          setOrders((ordersPage.items ?? []).filter((o) => o.customerId === customerId));
        } finally {
          if (!cancelled) setLoading(false);
        }
      })();
      return () => {
        cancelled = true;
      };
    }, [customerId])
  );

  if (loading || !customer) {
    return (
      <Screen scroll={false} style={styles.centered}>
        <ActivityIndicator color={colors.accent} size="large" />
      </Screen>
    );
  }

  const memberSince = new Date(customer.createdAt).toLocaleDateString(undefined, { month: 'long', year: 'numeric' });

  return (
    <Screen>
      <Pressable style={styles.backButton} onPress={() => navigation.goBack()}>
        <ArrowLeft color={colors.text} size={18} strokeWidth={2.75} />
      </Pressable>

      <View style={styles.identity}>
        <Avatar initials={initialsFor(customer.fullName)} size={64} />
        <Text style={styles.name}>{customer.fullName}</Text>
        <Text style={styles.meta}>
          Client since {memberSince} · {orders.length} order{orders.length === 1 ? '' : 's'}
        </Text>
      </View>

      <View style={styles.actionRow}>
        <View style={styles.actionButton}>
          <Button
            label="Call"
            variant="secondary"
            fullWidth
            disabled={!customer.phone}
            onPress={() => customer.phone && Linking.openURL(`tel:${customer.phone}`)}
          />
        </View>
        <View style={styles.actionButton}>
          <Button
            label="Message"
            fullWidth
            disabled={!customer.phone}
            onPress={() => customer.phone && Linking.openURL(`sms:${customer.phone}`)}
          />
        </View>
      </View>

      <Card>
        <Text style={styles.cardTitle}>Measurements</Text>
        <Text style={styles.notAvailable}>
          Measurement history isn't available here yet — the backend can record a new measurement set but has no
          endpoint to list past ones for a client. Worth adding before this card can show real data.
        </Text>
      </Card>

      <Card style={{ padding: 0 }}>
        <Text style={[styles.cardTitle, styles.cardTitlePadded]}>Orders</Text>
        {orders.length === 0 ? (
          <Text style={styles.emptyRow}>No orders yet.</Text>
        ) : (
          orders.map((order, index) => (
            <View key={order.id}>
              {index > 0 && <View style={styles.divider} />}
              <Pressable style={styles.orderRow} onPress={() => navigation.navigate('OrderDetail', { orderId: order.id })}>
                <View style={styles.rowBody}>
                  <Text style={styles.rowTitle}>{order.title}</Text>
                  <Text style={styles.rowSubtitle}>Due {new Date(order.deadlineDate).toLocaleDateString()}</Text>
                </View>
                <Pill label={order.status} tone={statusTone[order.status]} />
              </Pressable>
            </View>
          ))
        )}
      </Card>

      {!!customer.notes && (
        <Card style={styles.notesCard}>
          <Text style={styles.cardTitle}>Notes</Text>
          <Text style={styles.notesBody}>{customer.notes}</Text>
        </Card>
      )}
    </Screen>
  );
}

const styles = StyleSheet.create({
  centered: { alignItems: 'center', justifyContent: 'center' },
  backButton: {
    width: 36,
    height: 36,
    borderRadius: 18,
    borderWidth: 1.5,
    borderColor: colors.divider,
    alignItems: 'center',
    justifyContent: 'center',
    marginTop: spacing[2],
  },
  identity: { alignItems: 'center', gap: 4, marginTop: spacing[2] },
  name: { fontFamily: fontFamily.heading, fontSize: fontSize.sectionHeadingLg, color: colors.text },
  meta: { fontFamily: fontFamily.bodyRegular, fontSize: fontSize.secondary, color: colors.neutral600 },
  actionRow: { flexDirection: 'row', gap: spacing[2] },
  actionButton: { flex: 1 },
  cardTitle: { fontFamily: fontFamily.heading, fontSize: fontSize.cardTitle, color: colors.text },
  cardTitlePadded: { paddingHorizontal: spacing[3], paddingTop: spacing[3] },
  notAvailable: { fontFamily: fontFamily.bodyRegular, fontSize: fontSize.secondary, color: colors.neutral700, lineHeight: 18 },
  emptyRow: { fontFamily: fontFamily.bodyRegular, fontSize: fontSize.body, color: colors.neutral600, padding: spacing[3] },
  orderRow: { flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between', padding: spacing[3] },
  rowBody: { gap: 2, flex: 1 },
  rowTitle: { fontFamily: fontFamily.bodyBold, fontSize: fontSize.body, color: colors.text },
  rowSubtitle: { fontFamily: fontFamily.bodyRegular, fontSize: fontSize.secondary, color: colors.neutral600 },
  divider: { height: 1, backgroundColor: colors.divider, marginHorizontal: spacing[3] },
  notesCard: { backgroundColor: colors.accent2_100, borderWidth: 1, borderColor: colors.accent2_200 },
  notesBody: { fontFamily: fontFamily.bodyRegular, fontSize: fontSize.secondary, color: colors.accent2_900, lineHeight: 19 },
});
