import React from 'react';
import { Pressable, StyleSheet, View } from 'react-native';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { CalendarDays, House, Plus, Users } from 'lucide-react-native';
import { colors, shadow } from '../theme';
import { HomeScreen } from '../screens/HomeScreen';
import { ClientsListScreen } from '../screens/ClientsListScreen';
import { ScheduleScreen } from '../screens/ScheduleScreen';

export type MainTabParamList = {
  Home: undefined;
  Clients: undefined;
  // Never actually navigated to — see FabButton below. It exists purely so
  // the tab bar has a center slot to render the raised "+" button in.
  NewOrderTab: undefined;
  Schedule: undefined;
};

const Tab = createBottomTabNavigator<MainTabParamList>();

// The README's center FAB isn't a real tab (it opens a screen pushed onto
// the outer stack, not tab content) — this custom tabBarButton intercepts
// the press and stops the tab navigator from switching to a "NewOrderTab"
// screen that doesn't exist.
function FabButton({ navigation }: { navigation: any }) {
  return (
    <View style={styles.fabWrapper} pointerEvents="box-none">
      <Pressable
        style={styles.fab}
        onPress={() => navigation.getParent()?.navigate('NewOrder')}
      >
        <Plus color={colors.bg} size={26} strokeWidth={2.75} />
      </Pressable>
    </View>
  );
}

export function MainTabs() {
  return (
    <Tab.Navigator
      screenOptions={{
        headerShown: false,
        tabBarActiveTintColor: colors.accent700,
        tabBarInactiveTintColor: colors.neutral600,
        tabBarStyle: styles.tabBar,
        tabBarLabelStyle: styles.tabLabel,
      }}
    >
      <Tab.Screen
        name="Home"
        component={HomeScreen}
        options={{ tabBarIcon: ({ color }) => <House color={color} size={22} strokeWidth={2.75} /> }}
      />
      <Tab.Screen
        name="Clients"
        component={ClientsListScreen}
        options={{ tabBarIcon: ({ color }) => <Users color={color} size={22} strokeWidth={2.75} /> }}
      />
      <Tab.Screen
        name="NewOrderTab"
        component={HomeScreen}
        options={({ navigation }) => ({
          tabBarButton: () => <FabButton navigation={navigation} />,
        })}
        listeners={{
          tabPress: (e) => e.preventDefault(),
        }}
      />
      <Tab.Screen
        name="Schedule"
        component={ScheduleScreen}
        options={{ tabBarIcon: ({ color }) => <CalendarDays color={color} size={22} strokeWidth={2.75} /> }}
      />
    </Tab.Navigator>
  );
}

const styles = StyleSheet.create({
  tabBar: {
    backgroundColor: colors.neutral100,
    borderTopWidth: 1.5,
    borderTopColor: colors.divider,
    height: 64,
    paddingTop: 8,
  },
  tabLabel: { fontSize: 11, fontWeight: '700' },
  fabWrapper: { alignItems: 'center', top: -30 },
  fab: {
    width: 54,
    height: 54,
    borderRadius: 27,
    backgroundColor: colors.accent,
    alignItems: 'center',
    justifyContent: 'center',
    ...shadow.md,
  },
});
