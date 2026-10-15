import React, { ComponentProps } from "react";
import { colors, fontFamily, fontSize, spacing, radius, shadow  } from '../../theme/index';
import { View, Text, TextInput, Button, StyleSheet, Image, Pressable, TextStyle } from 'react-native';
import { Ionicons } from "@expo/vector-icons";

export default function AuthButton(
    { children, onPress, icon, color, backgroundColor } : 
    {children : React.ReactNode, 
    onPress? : () => void,
    icon? : ComponentProps<typeof Ionicons>['name'],
    color? : string,
    backgroundColor? : string
}
){
    return(
        <Pressable style={[styles.button, {backgroundColor}]}>
            {icon && <Ionicons name={icon} size={24} />}
            <Text style={{color}}>
                {children}
            </Text>
        </Pressable>
    )
}

const styles = StyleSheet.create({
    button: {
        display: 'flex',
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'center',
        padding: spacing[4],
        borderRadius: spacing[3],
        gap: spacing[4]
    }
})