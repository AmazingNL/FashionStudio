import React, { useCallback, useState } from 'react';
import { ActivityIndicator, Pressable, StyleSheet, Text, View } from 'react-native';
import { useFocusEffect, useNavigation } from '@react-navigation/native';
import { Card, Pill, Screen } from '../components';
import { customersApi, fittingsApi, ordersApi } from '../api';
import type { CustomerResponseDTO, FittingResponseDTO, OrderResponseDTO } from '../api';
import { colors, fontFamily, fontSize, spacing } from '../theme';

const DAYS_AHEAD = 7;

function isSameDay(a: Date, b: Date) {
  return a.getFullYear() === b.getFullYear() && a.getMonth() === b.getMonth() && a.getDate() === b.getDate();
}
function startOfDay(d: Date) {
  const copy = new Date(d);
  copy.setHours(0, 0, 0, 0);
  return copy;
}

export function ScheduleScreen() {
  const navigation = useNavigation<any>();
  const [loading, setLoading] = useState(true);
  const [selectedOffset, setSelectedOffset] = useState(0);
  const [fittings, setFittings] = useState<FittingResponseDTO[]>([]);
  const [orders, setOrders] = useState<OrderResponseDTO[]>([]);
  const [customersById, setCustomersById] = useState<Map<number, CustomerResponseDTO>>(new Map());

  useFocusEffect(
    useCallback(() => {
      let cancelled = false;
      (async () => {
        setLoading(true);
        try {
          const [fittingsPage, ordersPage, customersPage] = await Promise.all([
            fittingsApi.getFittings({ pageSize: 100 }),
            ordersApi.getOrders({ pageSize: 100 }),
            customersApi.getCustomers({ pageSize: 200 }),
          ]);
          if (cancelled) return;
          setFittings(fittingsPage.items ?? []);
          setOrders(ordersPage.items ?? []);
          setCustomersById(new Map((customersPage.items ?? []).map((c) => [c.id, c])));
        } finally {
          if (!cancelled) setLoading(false);
        }
      })();
      return () => {
        cancelled = true;
      };
    }, [])
  );

  const today = startOfDay(new Date());
  const days = Array.from({ length: DAYS_AHEAD }, (_, i) => {
    const d = new Date(today);
    d.setDate(d.getDate() + i);
    return d;
  });
  const selectedDay = days[selectedOffset];

  const dayFittings = fittings
    .filter((f) => isSameDay(new Date(f.fittingDate), selectedDay))
    .sort((a, b) => new Date(a.fittingDate).getTime() - new Date(b.fittingDate).getTime());
  const dayDueOrders = orders.filter((o) => isSameDay(new Date(o.deadlineDate), selectedDay));

  return (
    <Screen>
      <View style={styles.header}>
        <Text style={styles.title}>Schedule</Text>
        <Text style={styles.month}>{today.toLocaleDateString(undefined, { month: 'long', year: 'numeric' })}</Text>
      </View>

      <View style={styles.weekStrip}>
        {days.map((day, index) => {
          const isToday = index === 0;
          const isSelected = index === selectedOffset;
          const hasDeadline = orders.some((o) => isSameDay(new Date(o.deadlineDate), day));
          return (
            <Pressable key={day.toISOString()} onPress={() => setSelectedOffset(index)} style={styles.dayColumn}>
              <Text style={styles.dayLabel}>{day.toLocaleDateString(undefined, { weekday: 'short' }).toUpperCase()}</Text>
              <View
                style={[
                  styles.dayCircle,
                  isSelected && isToday && styles.dayCircleToday,
                  isSelected && !isToday && styles.dayCircleSelected,
                  !isSelected && hasDeadline && styles.dayCircleDeadline,
                ]}
              >
                <Text style={[styles.dayNumber, isSelected && isToday && styles.dayNumberToday]}>{day.getDate()}</Text>
              </View>
            </Pressable>
          );
        })}
      </View>

      {loading ? (
        <ActivityIndicator color={colors.accent} style={{ marginTop: spacing[6] }} />
      ) : dayFittings.length === 0 && dayDueOrders.length === 0 ? (
        <Card style={styles.emptyCard}>
          <Text style={styles.emptyTitle}>No appointments</Text>
          <Text style={styles.emptySubtitle}>Good sewing day</Text>
        </Card>
      ) : (
        <>
          {dayFittings.map((fitting) => {
            const customer = customersById.get(fitting.customerId);
            return (
              <Pressable
                key={`fitting-${fitting.id}`}
                onPress={() => navigation.navigate('OrderDetail', { orderId: fitting.orderId })}
              >
                <Card style={styles.appointmentCard}>
                  <View style={styles.appointmentRow}>
                    <Text style={styles.appointmentTime}>
                      {new Date(fitting.fittingDate).toLocaleTimeString(undefined, { hour: '2-digit', minute: '2-digit' })}
                    </Text>
                    <View style={{ flex: 1 }}>
                      <Text style={styles.appointmentTitle}>Fitting — {customer?.fullName ?? 'Client'}</Text>
                      <Text style={styles.appointmentSubtitle}>Order #{fitting.orderId}</Text>
                    </View>
                    <View style={styles.dot} />
                  </View>
                </Card>
              </Pressable>
            );
          })}
          {dayDueOrders.map((order) => (
            <Pressable key={`due-${order.id}`} onPress={() => navigation.navigate('OrderDetail', { orderId: order.id })}>
              <Card style={styles.dueCard}>
                <Pill label="DUE" tone="accent2" />
                <Text style={styles.appointmentTitle}>
                  Order #{order.id} — pickup, {customersById.get(order.customerId)?.fullName ?? 'Client'}
                </Text>
                <Text style={styles.appointmentSubtitle}>{order.title}</Text>
              </Card>
            </Pressable>
          ))}
        </>
      )}
    </Screen>
  );
}

const styles = StyleSheet.create({
  header: { flexDirection: 'row', alignItems: 'baseline', justifyContent: 'space-between', marginTop: spacing[3] },
  title: { fontFamily: fontFamily.heading, fontSize: fontSize.screenTitle, color: colors.text },
  month: { fontFamily: fontFamily.bodyBold, fontSize: fontSize.secondary, color: colors.accent700 },
  weekStrip: { flexDirection: 'row', justifyContent: 'space-between' },
  dayColumn: { alignItems: 'center', gap: 6 },
  dayLabel: { fontFamily: fontFamily.bodyBold, fontSize: 10, color: colors.neutral600 },
  dayCircle: {
    width: 40,
    height: 40,
    borderRadius: 20,
    alignItems: 'center',
    justifyContent: 'center',
  },
  dayCircleToday: { backgroundColor: colors.accent },
  dayCircleSelected: { backgroundColor: colors.neutral200 },
  dayCircleDeadline: { borderWidth: 2, borderColor: colors.accent2_500 },
  dayNumber: { fontFamily: fontFamily.bodyBold, fontSize: fontSize.body, color: colors.text },
  dayNumberToday: { color: colors.bg },
  emptyCard: {
    borderWidth: 1.5,
    borderColor: colors.neutral400,
    borderStyle: 'dashed',
    backgroundColor: 'transparent',
    alignItems: 'center',
    paddingVertical: spacing[6],
  },
  emptyTitle: { fontFamily: fontFamily.bodyBold, fontSize: fontSize.body, color: colors.text },
  emptySubtitle: { fontFamily: fontFamily.bodyRegular, fontSize: fontSize.secondary, color: colors.neutral600 },
  appointmentCard: {},
  appointmentRow: { flexDirection: 'row', alignItems: 'center', gap: spacing[3] },
  appointmentTime: { fontFamily: fontFamily.bodyBold, fontSize: fontSize.listTitle, color: colors.text, width: 60 },
  appointmentTitle: { fontFamily: fontFamily.bodyBold, fontSize: fontSize.listTitle, color: colors.text },
  appointmentSubtitle: { fontFamily: fontFamily.bodyRegular, fontSize: fontSize.secondary, color: colors.neutral600 },
  dot: { width: 10, height: 10, borderRadius: 5, backgroundColor: colors.accent },
  dueCard: { backgroundColor: colors.accent2_100, gap: 4 },
});
