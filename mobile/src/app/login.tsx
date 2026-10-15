import React from 'react';

import { View, Text, StyleSheet } from 'react-native'; 
import { colors, spacing, radius, shadow } from '../theme'; 
import LoginScreen from '../screens/LoginScreen';

export default function App() {
    return (
        <>
            <View style={styles.container}>
                <LoginScreen />
            </View>
        </>

    )
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: colors.bg,
        padding: spacing[4],
        height: '100%',
    },


});