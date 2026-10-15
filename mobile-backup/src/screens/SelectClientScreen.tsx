import React, { useEffect, useState } from 'react';
import { ActivityIndicator, Pressable, StyleSheet, Text, View } from 'react-native';
import { useNavigation } from '@react-navigation/native';
import { ArrowLeft } from 'lucide-react-native';
import { Screen, TextField } from '../components';
import { customersApi } from '../api';
import type { CustomerResponseDTO } from '../api';
import { colors, fontFamily, fontSize, spacing } from '../theme';

// Pops back to whichever screen pushed this one, passing the pick back via
// route params merged onto that screen — the standard React Navigation
// pattern for "return a value" flows without a dedicated state library.
export function SelectClientScreen() {
  const navigation = useNavigation<any>();
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [customers, setCustomers] = useState<CustomerResponseDTO[]>([]);

  useEffect(() => {
    (async () => {
      const page = await customersApi.getCustomers({ pageSize: 200, sortBy: 'FullName' });
      setCustomers((page.items ?? []).filter((c) => c.isActive));
      setLoading(false);
    })();
  }, []);

  const filtered = customers.filter((c) => c.fullName.toLowerCase().includes(search.trim().toLowerCase()));

  return (
    <Screen>
      <Pressable style={styles.backButton} onPress={() => navigation.goBack()}>
        <ArrowLeft color={colors.text} size={18} strokeWidth={2.75} />
      </Pressable>
      <Text style={styles.title}>Select client</Text>
      <TextField label="" value={search} onChangeText={setSearch} placeholder="Search…" />

      {loading ? (
        <ActivityIndicator color={colors.accent} style={{ marginTop: spacing[6] }} />
      ) : (
        filtered.map((customer) => (
          <Pressable
            key={customer.id}
            style={styles.row}
            onPress={() =>
              navigation.navigate({
                name: 'NewOrder',
                params: { selectedCustomerId: customer.id, selectedCustomerName: customer.fullName },
                merge: true,
              })
            }
          >
            <Text style={styles.rowTitle}>{customer.fullName}</Text>
          </Pressable>
        ))
      )}
    </Screen>
  );
}

const styles = StyleSheet.create({
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
  title: { fontFamily: fontFamily.heading, fontSize: fontSize.sectionHeadingLg, color: colors.text },
  row: { paddingVertical: 14, borderBottomWidth: 1, borderBottomColor: colors.divider },
  rowTitle: { fontFamily: fontFamily.bodyBold, fontSize: fontSize.listTitle, color: colors.text },
});
