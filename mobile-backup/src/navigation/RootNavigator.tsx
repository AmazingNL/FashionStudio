import React from 'react';
import { ActivityIndicator, View } from 'react-native';
import { NavigationContainer, DefaultTheme } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { useAuth } from '../context/AuthContext';
import { colors } from '../theme';
import { AuthScreen } from '../screens/AuthScreen';
import { NeedsWorkSpaceScreen } from '../screens/NeedsWorkSpaceScreen';
import { MainTabs } from './MainTabs';
import { ClientProfileScreen } from '../screens/ClientProfileScreen';
import { NewClientScreen } from '../screens/NewClientScreen';
import { SelectClientScreen } from '../screens/SelectClientScreen';
import { OrderDetailScreen } from '../screens/OrderDetailScreen';
import { NewOrderScreen } from '../screens/NewOrderScreen';
import { RecordPaymentScreen } from '../screens/RecordPaymentScreen';

export type RootStackParamList = {
  Auth: undefined;
  NeedsWorkSpace: undefined;
  MainTabs: undefined;
  ClientProfile: { customerId: number };
  NewClient: undefined;
  SelectClient: undefined;
  OrderDetail: { orderId: number };
  NewOrder: { selectedCustomerId?: number; selectedCustomerName?: string } | undefined;
  RecordPayment: { orderId: number; balance: number };
};

const Stack = createNativeStackNavigator<RootStackParamList>();

const navTheme = {
  ...DefaultTheme,
  colors: { ...DefaultTheme.colors, background: colors.bg },
};

export function RootNavigator() {
  const { status } = useAuth();

  if (status === 'loading') {
    return (
      <View style={{ flex: 1, alignItems: 'center', justifyContent: 'center', backgroundColor: colors.bg }}>
        <ActivityIndicator color={colors.accent} size="large" />
      </View>
    );
  }

  return (
    <NavigationContainer theme={navTheme}>
      <Stack.Navigator screenOptions={{ headerShown: false }}>
        {status === 'signedOut' ? (
          <Stack.Screen name="Auth" component={AuthScreen} />
        ) : status === 'needsWorkSpace' ? (
          <Stack.Screen name="NeedsWorkSpace" component={NeedsWorkSpaceScreen} />
        ) : (
          <>
            <Stack.Screen name="MainTabs" component={MainTabs} />
            <Stack.Screen name="ClientProfile" component={ClientProfileScreen} />
            <Stack.Screen name="NewClient" component={NewClientScreen} options={{ presentation: 'modal' }} />
            <Stack.Screen name="SelectClient" component={SelectClientScreen} options={{ presentation: 'modal' }} />
            <Stack.Screen name="OrderDetail" component={OrderDetailScreen} />
            <Stack.Screen name="NewOrder" component={NewOrderScreen} options={{ presentation: 'modal' }} />
            <Stack.Screen name="RecordPayment" component={RecordPaymentScreen} options={{ presentation: 'modal' }} />
          </>
        )}
      </Stack.Navigator>
    </NavigationContainer>
  );
}
