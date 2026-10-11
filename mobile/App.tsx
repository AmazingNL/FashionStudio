import React from 'react';
import Login from './src/screens/Login';
import SignupScreen from './src/screens/SignupScreen';

import { View, Text, StyleSheet } from 'react-native'; 
import { colors, spacing, radius, shadow } from './src/theme'; 

export default function App() {
    return (
        <>


            <View style={styles.container}>
                <SignupScreen />
            </View>
        </>

    )
}

//<View style={styles.container}>
//    <Text style={styles.title}>Fashion Studio!</Text>
//    <Text style={styles.subtitle}>Welcome to the world of fashion!</Text>
//</View>
//<View>
//    <Login />
//</View>
const styles = StyleSheet.create({
    container: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
        backgroundColor: colors.bg,
        padding: spacing[4],
    },

    title: {
        fontSize: 24,
        fontWeight: 'bold',
        color: colors.accent,
        marginBottom: spacing[2],
    },

    subtitle: {
        fontSize: 18,
        color: colors.text,
        marginBottom: spacing[4],
    },

});




