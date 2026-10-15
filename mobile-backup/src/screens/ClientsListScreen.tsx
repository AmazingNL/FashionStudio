import React, { useCallback, useState } from 'react';
import { ActivityIndicator, Pressable, StyleSheet, Text, View } from 'react-native';
import { useFocusEffect, useNavigation } from '@react-navigation/native';
import { ChevronRight, Plus, Search } from 'lucide-react-native';
import { Avatar, Card, Screen, TextField } from '../components';
import { customersApi } from '../api';
import type { CustomerResponseDTO } from '../api';
import { colors, fontFamily, fontSize, spacing } from '../theme';

function initialsFor(name: string) {
  const parts = name.trim().split(/\s+/);
  return ((parts[0]?.[0] ?? '') + (parts[1]?.[0] ?? '')).toUpperCase() || '?';
}

export function ClientsListScreen() {
  const navigation = useNavigation<any>();
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [customers, setCustomers] = useState<CustomerResponseDTO[]>([]);

  useFocusEffect(
    useCallback(() => {
      let cancelled = false;
      (async () => {
        setLoading(true);
        try {
          const page = await customersApi.getCustomers({ pageSize: 200, sortBy: 'FullName' });
          if (!cancelled) setCustomers(page.items ?? []);
        } finally {
          if (!cancelled) setLoading(false);
        }
      })();
      return () => {
        cancelled = true;
      };
    }, [])
  );

  const filtered = customers.filter((c) => c.fullName.toLowerCase().includes(search.trim().toLowerCase()));

  return (
    <Screen>
      <View style={styles.header}>
        <Text style={styles.title}>Clients</Text>
        <Pressable style={styles.addButton} onPress={() => navigation.navigate('NewClient')}>
          <Plus color={colors.bg} size={20} strokeWidth={2.75} />
        </Pressable>
      </View>

      <View style={styles.searchBox}>
        <Search color={colors.neutral500} size={17} strokeWidth={2.75} />
        <TextField
          label=""
          value={search}
          onChangeText={setSearch}
          placeholder="Search by name…"
          style={styles.searchInput}
        />
      </View>

      {loading ? (
        <ActivityIndicator color={colors.accent} style={{ marginTop: spacing[6] }} />
      ) : filtered.length === 0 ? (
        <Text style={styles.empty}>{customers.length === 0 ? 'No clients yet.' : 'No matches.'}</Text>
      ) : (
        <Card style={{ padding: 0 }}>
          {filtered.map((customer, index) => (
            <View key={customer.id}>
              {index > 0 && <View style={styles.divider} />}
              <Pressable
                style={styles.row}
                onPress={() => navigation.navigate('ClientProfile', { customerId: customer.id })}
              >
                <Avatar
                  initials={initialsFor(customer.fullName)}
                  tone={(['accent', 'accent2', 'neutral'] as const)[index % 3]}
                />
                <View style={styles.rowBody}>
                  <Text style={styles.rowTitle}>{customer.fullName}</Text>
                  <Text style={styles.rowSubtitle}>
                    {customer.isActive ? customer.phone || 'No phone on file' : 'Inactive'}
                  </Text>
                </View>
                <ChevronRight color={colors.neutral600} size={18} strokeWidth={2.75} />
              </Pressable>
            </View>
          ))}
        </Card>
      )}

      {!loading && customers.length > 0 && (
        <Text style={styles.footer}>
          {customers.length} client{customers.length === 1 ? '' : 's'}
        </Text>
      )}
    </Screen>
  );
}

const styles = StyleSheet.create({
  header: { flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between', marginTop: spacing[3] },
  title: { fontFamily: fontFamily.heading, fontSize: fontSize.screenTitle, color: colors.text },
  addButton: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: colors.accent,
    alignItems: 'center',
    justifyContent: 'center',
  },
  searchBox: { flexDirection: 'row', alignItems: 'center', gap: spacing[2] },
  searchInput: { flex: 1 },
  empty: {
    fontFamily: fontFamily.bodyRegular,
    fontSize: fontSize.body,
    color: colors.neutral600,
    textAlign: 'center',
    marginTop: spacing[6],
  },
  row: { flexDirection: 'row', alignItems: 'center', paddingVertical: 13, paddingHorizontal: 18, gap: spacing[3] },
  rowBody: { flex: 1, gap: 2 },
  rowTitle: { fontFamily: fontFamily.bodyBold, fontSize: fontSize.listTitle, color: colors.text },
  rowSubtitle: { fontFamily: fontFamily.bodyRegular, fontSize: fontSize.secondary, color: colors.neutral600 },
  divider: { height: 1, backgroundColor: colors.divider, marginHorizontal: 18 },
  footer: { fontFamily: fontFamily.bodyRegular, fontSize: fontSize.secondary, color: colors.neutral600, textAlign: 'center' },
});
