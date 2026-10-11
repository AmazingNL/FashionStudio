import React, { useCallback, useState } from 'react';
import { ActivityIndicator, Pressable, StyleSheet, Text, View } from 'react-native';
import { useFocusEffect, useNavigation, useRoute } from '@react-navigation/native';
import { ArrowLeft, Check } from 'lucide-react-native';
import { Card, Pill, Screen } from '../components';
import type { PillTone } from '../components';
import { customersApi, ordersApi, paymentsApi } from '../api';
import type { CustomerResponseDTO, OrderResponseDTO, OrderStatus, PaymentResponseDTO } from '../api';
import { colors, fontFamily, fontSize, spacing } from '../theme';

const TIMELINE_STEPS: { key: OrderStatus; label: string }[] = [
  { key: 'New', label: 'Placed' },
  { key: 'InProgress', label: 'In progress' },
  { key: 'Fitting', label: 'Fitting' },
  { key: 'Ready', label: 'Ready' },
];

const statusTone: Record<OrderStatus, PillTone> = {
  New: 'neutral',
  InProgress: 'neutral',
  Fitting: 'accent2',
  Ready: 'accent',
  Delivered: 'neutral',
  Cancelled: 'neutral',
  Rejected: 'neutral',
};

function currencySymbol(code: string) {
  return { NGN: '₦', USD: '$', GBP: '£', EUR: '€' }[code] ?? code + ' ';
}

export function OrderDetailScreen() {
  const navigation = useNavigation<any>();
  const route = useRoute<any>();
  const orderId: number = route.params.orderId;

  const [loading, setLoading] = useState(true);
  const [order, setOrder] = useState<OrderResponseDTO | null>(null);
  const [customer, setCustomer] = useState<CustomerResponseDTO | null>(null);
  const [payments, setPayments] = useState<PaymentResponseDTO[]>([]);

  const load = useCallback(async () => {
    const orderResult = await ordersApi.getOrderById(orderId);
    const [customerResult, paymentsPage] = await Promise.all([
      customersApi.getCustomerById(orderResult.customerId),
      paymentsApi.getPayments({ pageSize: 200 }),
    ]);
    setOrder(orderResult);
    setCustomer(customerResult);
    setPayments((paymentsPage.items ?? []).filter((p) => p.orderId === orderId));
  }, [orderId]);

  useFocusEffect(
    useCallback(() => {
      let cancelled = false;
      (async () => {
        setLoading(true);
        try {
          await load();
        } finally {
          if (!cancelled) setLoading(false);
        }
      })();
      return () => {
        cancelled = true;
      };
    }, [load])
  );

  if (loading || !order || !customer) {
    return (
      <Screen scroll={false} style={styles.centered}>
        <ActivityIndicator color={colors.accent} size="large" />
      </Screen>
    );
  }

  const symbol = currencySymbol(order.currency);
  const amountPaid = payments.reduce((sum, p) => sum + p.amount, 0);
  const totalDue = order.quotedPrice - order.discount;
  const balance = totalDue - amountPaid;
  const progress = totalDue > 0 ? Math.min(1, amountPaid / totalDue) : 0;

  const isLinearStatus = TIMELINE_STEPS.some((s) => s.key === order.status) || order.status === 'Delivered';
  const currentStepIndex =
    order.status === 'Delivered'
      ? TIMELINE_STEPS.length
      : TIMELINE_STEPS.findIndex((s) => s.key === order.status);

  return (
    <Screen>
      <Pressable style={styles.backButton} onPress={() => navigation.goBack()}>
        <ArrowLeft color={colors.text} size={18} strokeWidth={2.75} />
      </Pressable>

      <View style={styles.headerRow}>
        <View style={{ flex: 1 }}>
          <Text style={styles.title}>{order.title}</Text>
          <Text style={styles.subtitle}>
            {customer.fullName} · due {new Date(order.deadlineDate).toLocaleDateString()}
          </Text>
        </View>
        <Pill label={order.status} tone={statusTone[order.status]} />
      </View>

      {isLinearStatus && (
        <Card>
          <Text style={styles.cardTitle}>Status</Text>
          <View style={styles.timeline}>
            {TIMELINE_STEPS.map((step, index) => {
              const done = index < currentStepIndex;
              const current = index === currentStepIndex;
              return (
                <View key={step.key} style={styles.timelineStep}>
                  <View style={styles.timelineIndicatorRow}>
                    <View
                      style={[
                        styles.timelineDot,
                        done && styles.timelineDotDone,
                        current && styles.timelineDotCurrent,
                      ]}
                    >
                      {done && <Check color={colors.bg} size={14} strokeWidth={3} />}
                    </View>
                    {index < TIMELINE_STEPS.length - 1 && (
                      <View style={[styles.timelineConnector, done && styles.timelineConnectorDone]} />
                    )}
                  </View>
                  <Text style={[styles.timelineLabel, current && styles.timelineLabelCurrent]}>{step.label}</Text>
                </View>
              );
            })}
          </View>
        </Card>
      )}

      <Card>
        <View style={styles.paymentHeader}>
          <Text style={styles.cardTitle}>Payment</Text>
          <Pressable onPress={() => navigation.navigate('RecordPayment', { orderId: order.id, balance })}>
            <Text style={styles.recordLink}>Record payment</Text>
          </Pressable>
        </View>
        <Text style={styles.amountPaid}>
          {symbol}
          {amountPaid.toLocaleString()}
        </Text>
        <Text style={styles.amountOfTotal}>
          of {symbol}
          {totalDue.toLocaleString()} paid
        </Text>
        <View style={styles.progressTrack}>
          <View style={[styles.progressFill, { width: `${progress * 100}%` }]} />
        </View>
        <Text style={styles.balanceLine}>
          Balance {symbol}
          {balance.toLocaleString()} · due at pickup
        </Text>
      </Card>

      {!!order.description && (
        <Card style={styles.notesCard}>
          <Text style={styles.cardTitle}>Notes</Text>
          <Text style={styles.notesBody}>{order.description}</Text>
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
  headerRow: { flexDirection: 'row', alignItems: 'flex-start', gap: spacing[2] },
  title: { fontFamily: fontFamily.heading, fontSize: fontSize.sectionHeadingLg, color: colors.text },
  subtitle: { fontFamily: fontFamily.bodyRegular, fontSize: fontSize.secondary, color: colors.neutral600 },
  cardTitle: { fontFamily: fontFamily.heading, fontSize: fontSize.cardTitle, color: colors.text },
  timeline: { flexDirection: 'row' },
  timelineStep: { flex: 1, alignItems: 'center', gap: 6 },
  timelineIndicatorRow: { flexDirection: 'row', alignItems: 'center', width: '100%' },
  timelineDot: {
    width: 26,
    height: 26,
    borderRadius: 13,
    borderWidth: 2.5,
    borderColor: colors.neutral400,
    alignItems: 'center',
    justifyContent: 'center',
    marginLeft: 'auto',
    marginRight: 'auto',
  },
  timelineDotDone: { backgroundColor: colors.accent2_500, borderColor: colors.accent2_500 },
  timelineDotCurrent: { backgroundColor: colors.accent, borderColor: colors.accent200, borderWidth: 4 },
  timelineConnector: { flex: 1, height: 2.5, backgroundColor: colors.neutral300, marginTop: -13 },
  timelineConnectorDone: { backgroundColor: colors.accent2_500 },
  timelineLabel: { fontFamily: fontFamily.bodyRegular, fontSize: fontSize.label, color: colors.neutral600, textAlign: 'center' },
  timelineLabelCurrent: { fontFamily: fontFamily.bodyBold, color: colors.accent700 },
  paymentHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center' },
  recordLink: { fontFamily: fontFamily.bodyBold, fontSize: fontSize.secondary, color: colors.accent700 },
  amountPaid: { fontFamily: fontFamily.heading, fontSize: 22, color: colors.text },
  amountOfTotal: { fontFamily: fontFamily.bodyRegular, fontSize: fontSize.secondary, color: colors.neutral600, marginTop: -4 },
  progressTrack: { height: 10, borderRadius: 5, backgroundColor: colors.neutral200, overflow: 'hidden' },
  progressFill: { height: '100%', backgroundColor: colors.accent, borderRadius: 5 },
  balanceLine: { fontFamily: fontFamily.bodyRegular, fontSize: fontSize.secondary, color: colors.neutral600 },
  notesCard: { backgroundColor: colors.accent2_100, borderWidth: 1, borderColor: colors.accent2_200 },
  notesBody: { fontFamily: fontFamily.bodyRegular, fontSize: fontSize.secondary, color: colors.accent2_900, lineHeight: 19 },
});
