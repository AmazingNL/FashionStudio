import React, { useCallback, useState } from 'react';
import { ActivityIndicator, Pressable, RefreshControl, StyleSheet, Text, View } from 'react-native';
import { useFocusEffect, useNavigation } from '@react-navigation/native';
import { ChevronRight } from 'lucide-react-native';
import { Avatar, Button, Card, Pill, Screen } from '../components';
import { useAuth } from '../context/AuthContext';
import { customersApi, fittingsApi, ordersApi } from '../api';
import type { CustomerResponseDTO, FittingResponseDTO, OrderResponseDTO } from '../api';
import { colors, fontFamily, fontSize, spacing } from '../theme';

const DUE_SOON_DAYS = 3;
const OPEN_STATUSES = ['New', 'InProgress', 'Fitting', 'Ready'] as const;

function isSameDay(a: Date, b: Date) {
  return a.getFullYear() === b.getFullYear() && a.getMonth() === b.getMonth() && a.getDate() === b.getDate();
}

function initialsFor(name: string) {
  const parts = name.trim().split(/\s+/);
  return ((parts[0]?.[0] ?? '') + (parts[1]?.[0] ?? '')).toUpperCase() || 'FS';
}

export function HomeScreen() {
  const { user, workSpace } = useAuth();
  const navigation = useNavigation<any>();

  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [fittings, setFittings] = useState<FittingResponseDTO[]>([]);
  const [orders, setOrders] = useState<OrderResponseDTO[]>([]);
  const [customersById, setCustomersById] = useState<Map<number, CustomerResponseDTO>>(new Map());

  const load = useCallback(async () => {
    // MVP-scale simplification: pull one large page of each rather than
    // building real pagination/filtering — fine for a solo tailor's volume,
    // won't scale past a few hundred records.
    const [fittingsPage, ordersPage, customersPage] = await Promise.all([
      fittingsApi.getFittings({ pageSize: 100 }),
      ordersApi.getOrders({ pageSize: 100 }),
      customersApi.getCustomers({ pageSize: 200 }),
    ]);
    setFittings(fittingsPage.items ?? []);
    setOrders(ordersPage.items ?? []);
    setCustomersById(new Map((customersPage.items ?? []).map((c) => [c.id, c])));
  }, []);

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

  async function onRefresh() {
    setRefreshing(true);
    try {
      await load();
    } finally {
      setRefreshing(false);
    }
  }

  if (loading) {
    return (
      <Screen scroll={false} style={styles.centered}>
        <ActivityIndicator color={colors.accent} size="large" />
      </Screen>
    );
  }

  const today = new Date();
  const todaysFittings = fittings
    .filter((f) => isSameDay(new Date(f.fittingDate), today))
    .sort((a, b) => new Date(a.fittingDate).getTime() - new Date(b.fittingDate).getTime());

  const dueSoonCutoff = new Date(today);
  dueSoonCutoff.setDate(dueSoonCutoff.getDate() + DUE_SOON_DAYS);
  const dueSoonOrders = orders.filter(
    (o) => OPEN_STATUSES.includes(o.status as any) && new Date(o.deadlineDate) <= dueSoonCutoff
  );

  const sewingCount = orders.filter((o) => o.status === 'InProgress').length;
  const fittingCount = orders.filter((o) => o.status === 'Fitting').length;

  return (
    <Screen refreshControl={<RefreshControl refreshing={refreshing} onRefresh={onRefresh} tintColor={colors.accent} />}>
      <View style={styles.header}>
        <Text style={styles.title}>FashionStudio</Text>
        <Avatar initials={user ? initialsFor(user.userName) : 'FS'} />
      </View>
      <Text style={styles.subline}>
        {today.toLocaleDateString(undefined, { weekday: 'long', month: 'short', day: 'numeric' })}
        {workSpace ? ` · ${workSpace.name}` : ''}
      </Text>

      <Text style={styles.sectionLabel}>TODAY</Text>
      <Card style={{ padding: 0 }}>
        {todaysFittings.length === 0 && dueSoonOrders.length === 0 ? (
          <Text style={styles.emptyRow}>Nothing on today. Enjoy the quiet.</Text>
        ) : (
          <>
            {todaysFittings.map((fitting, index) => {
              const customer = customersById.get(fitting.customerId);
              return (
                <Row
                  key={`fitting-${fitting.id}`}
                  showDivider={index > 0 || dueSoonOrders.length > 0}
                  leading={
                    <Text style={styles.rowTime}>
                      {new Date(fitting.fittingDate).toLocaleTimeString(undefined, {
                        hour: '2-digit',
                        minute: '2-digit',
                      })}
                    </Text>
                  }
                  title={`Fitting — ${customer?.fullName ?? 'Client'}`}
                  subtitle={`Order #${fitting.orderId}`}
                  onPress={() => navigation.navigate('OrderDetail', { orderId: fitting.orderId })}
                />
              );
            })}
            {dueSoonOrders.map((order, index) => (
              <Row
                key={`due-${order.id}`}
                showDivider={index > 0}
                leading={<Pill label="DUE" tone="accent2" />}
                title={`Order #${order.id} — ${order.title}`}
                subtitle={`Due ${new Date(order.deadlineDate).toLocaleDateString(undefined, { weekday: 'short' })}`}
                onPress={() => navigation.navigate('OrderDetail', { orderId: order.id })}
              />
            ))}
          </>
        )}
      </Card>

      <Text style={styles.sectionLabel}>ORDERS IN PROGRESS</Text>
      <View style={styles.statRow}>
        <StatTile value={sewingCount} label="Sewing" />
        <StatTile value={fittingCount} label="Fitting" />
        <StatTile value={dueSoonOrders.length} label="Due soon" tone="accent2" />
      </View>

      <View style={styles.actionRow}>
        <Button label="+ New order" onPress={() => navigation.navigate('NewOrder')} fullWidth />
      </View>
    </Screen>
  );
}

function Row({
  leading,
  title,
  subtitle,
  onPress,
  showDivider,
}: {
  leading: React.ReactNode;
  title: string;
  subtitle: string;
  onPress: () => void;
  showDivider: boolean;
}) {
  return (
    <View>
      {showDivider && <View style={styles.divider} />}
      <Pressable style={styles.row} onPress={onPress}>
        <View style={styles.rowLeading}>{leading}</View>
        <View style={styles.rowBody}>
          <Text style={styles.rowTitle}>{title}</Text>
          <Text style={styles.rowSubtitle}>{subtitle}</Text>
        </View>
        <ChevronRight color={colors.neutral600} size={18} strokeWidth={2.75} />
      </Pressable>
    </View>
  );
}

function StatTile({ value, label, tone }: { value: number; label: string; tone?: 'accent2' }) {
  return (
    <View style={[styles.statTile, tone === 'accent2' && styles.statTileAccent2]}>
      <Text style={[styles.statNumber, tone === 'accent2' && styles.statNumberAccent2]}>{value}</Text>
      <Text style={styles.statLabel}>{label}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  centered: { alignItems: 'center', justifyContent: 'center' },
  header: { flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between', marginTop: spacing[3] },
  title: { fontFamily: fontFamily.heading, fontSize: fontSize.sectionHeadingLg, color: colors.text },
  subline: { fontFamily: fontFamily.bodyRegular, fontSize: fontSize.secondary, color: colors.neutral600 },
  sectionLabel: {
    fontFamily: fontFamily.bodyBold,
    fontSize: fontSize.label,
    letterSpacing: 1.6,
    color: colors.neutral600,
    marginTop: spacing[2],
  },
  emptyRow: {
    fontFamily: fontFamily.bodyRegular,
    fontSize: fontSize.body,
    color: colors.neutral600,
    padding: spacing[4],
    textAlign: 'center',
  },
  row: { flexDirection: 'row', alignItems: 'center', paddingVertical: 14, paddingHorizontal: 18, gap: spacing[3] },
  rowLeading: { width: 48, alignItems: 'flex-start' },
  rowTime: { fontFamily: fontFamily.bodyBold, fontSize: fontSize.listTitle },
  rowBody: { flex: 1, gap: 2 },
  rowTitle: { fontFamily: fontFamily.bodyBold, fontSize: fontSize.listTitle, color: colors.text },
  rowSubtitle: { fontFamily: fontFamily.bodyRegular, fontSize: fontSize.secondary, color: colors.neutral600 },
  divider: { height: 1, backgroundColor: colors.divider, marginHorizontal: 18 },
  statRow: { flexDirection: 'row', gap: spacing[2] },
  statTile: {
    flex: 1,
    backgroundColor: colors.neutral100,
    borderRadius: 16,
    paddingVertical: 14,
    paddingHorizontal: 8,
    alignItems: 'center',
    gap: 2,
  },
  statTileAccent2: { backgroundColor: colors.accent2_200 },
  statNumber: { fontFamily: fontFamily.heading, fontSize: fontSize.statNumber, color: colors.text },
  statNumberAccent2: { color: colors.accent2_800 },
  statLabel: { fontFamily: fontFamily.bodyRegular, fontSize: fontSize.secondary, color: colors.neutral600 },
  actionRow: { marginTop: spacing[2] },
});
