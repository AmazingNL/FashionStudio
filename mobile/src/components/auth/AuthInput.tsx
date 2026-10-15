import { ComponentProps} from 'react';
import { colors, fontFamily, fontSize, spacing, radius, shadow  } from '../../theme/index';
import { View, TextInput, StyleSheet, TextInputProps } from 'react-native';
import { Ionicons } from '@expo/vector-icons'
import clsx from 'clsx';

interface AuthInputProps extends TextInputProps {
    icon? : ComponentProps<typeof Ionicons>['name']
    iconColor? : string
}

export default function AuthInput( 
    {   icon, 
        iconColor = colors.neutral700,  
        ...props
    } : AuthInputProps
){

    return(
        <View style={styles.input_container}>
            {
                icon && 
                <Ionicons 
                    name={icon} 
                    size={24} 
                    color={iconColor} 
                />
            }
            <TextInput
                { ...props }
            />
        </View>
    )
}

const styles = StyleSheet.create({

    input_container : {
        display: 'flex',
        flexDirection: 'row',
        gap: 10,
        borderWidth: 1,
        padding: 15,
        borderRadius: 11,
        width: '100%',
        marginVertical: 5,
    },

})