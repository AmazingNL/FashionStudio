import React from "react";
import { colors, fontFamily, fontSize, spacing, radius, shadow  } from '../../theme/index';
import { View, Text, TextInput, Button, StyleSheet, Image } from 'react-native';

export default function AuthHeader(){
    return(
        <View style={styles.container}>
            <View>
                <View style={styles.title_container}>
                    <Text style={styles.fashion}>Fashion</Text>
                    <Text style={styles.title}>Studio</Text>
                </View>
                <Text style={styles.subtitle}>Design. Create. Dress with Purpose</Text>
            </View>
                            <Image
                    source={require('../../../assets/Container.png')}
                    style={styles.image}
                />
            <View>

            </View>
        </View>
    )
}

const styles = StyleSheet.create({
    container: {
        marginTop: 50,
        display: 'flex',
        flexDirection: 'row',
    },

    title_container: {
        display: 'flex',
        flexDirection: 'row',
        gap: spacing[1]
    },

    title: {
        fontSize: fontSize.screenTitle,
        fontFamily: fontFamily.heading,
    },

    fashion: {
        fontSize: fontSize.screenTitle,
        fontFamily: fontFamily.heading,
        fontWeight: 'bold',
        color: colors.accent
    },
    subtitle: {
        fontSize: fontSize.caption,
        color: colors.neutral700,
    },
    image: {
        width: 150,
        height: 150,
        left: -60,
        zIndex: 80
    },
});