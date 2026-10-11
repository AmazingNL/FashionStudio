import React from 'react';
import { colors, fontFamily, fontSize, spacing, radius, shadow  } from '../theme/index';
import { View, Text, TextInput, Button, StyleSheet, Image } from 'react-native';

export default function SignupScreen() {

    return (
        <>
            <View style={styles.container}>
                <Text style={styles.title}>Fashion Studio</Text>
                <Text style={styles.subtitle}>Design. Create. Dress with Purpose</Text>

                <Image
                    source={require('../../assets/Container.png')}
                    style={{ width: 100, height: 100 }}
                />
            </View>
        </>
        
    )
}

const styles = StyleSheet.create({
    container: {
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'flex-start',
        backgroundColor: '#fff',
        padding: 16,
    },
    title: {
        fontSize: fontSize.screenTitle,

        fontWeight: 'bold',
        marginBottom: 8,
    },
    subtitle: {
        fontSize: 18,
        color: '#666',
        marginBottom: 16,
    },
    image: {
        width: 100,
        height: 100,
    },
});