import React from 'react';
import { colors, fontFamily, fontSize, spacing, radius, shadow  } from '../theme/index';
import { View, Text, TextInput, Button, StyleSheet, Image } from 'react-native';
import AuthHeader from '../components/auth/AuthHeader';
import AuthButton from '../components/auth/AuthButton';
import AuthInput from '../components/auth/AuthInput';
import AuthDivider from '../components/auth/AthDivider';
import AuthLink from '../components/auth/authLink';

export default function SignupScreen() {

    return (
        <View style={styles.container}>
            <View>
                <AuthHeader />
            </View>

            <View style={styles.title}>
                <Text 
                    style={{
                        fontSize: fontSize.sectionHeadingLg
                    }}>
                        Create your account
                </Text>
                <Text 
                    style={{
                        fontSize: fontSize.secondary, 
                        fontFamily: fontFamily.bodyRegular
                    }}>
                        Join FashionStudio and start bringing your ideas to life.
                </Text>
            </View>

            <View>
                <AuthInput
                    placeholder='Your name, Eg. Amara Erdoo'
                    icon='person'
                    iconColor={colors.accent2}
                />
                <AuthInput
                    keyboardType='email-address'
                    placeholder='email: eg james@gmail.com'
                    icon='mail-outline'
                    iconColor={colors.accent}
                />
                <AuthInput
                    placeholder='Enter your password'
                    icon='key'
                    secureTextEntry
                    iconColor={colors.accent}
                />
                <AuthInput
                    placeholder='Re-enter your password'
                    icon='key'
                    secureTextEntry
                    iconColor={colors.accent}
                />
            </View>

            <View style={styles.policy}>
                <Text style={styles.policy_text}>I agree to the Terms of Service and Privacy Policy .</Text>
            </View>

            <View>
                <AuthButton
                    backgroundColor={colors.accent}
                    color={colors.bg}
                >
                    Create account
                </AuthButton>
            </View>

            <View>
                <AuthDivider />
            </View>

            <View style={styles.signup_buttons}>
                <AuthButton
                    backgroundColor={colors.neutral300}
                    icon='logo-apple'
                >
                    Continue with Apple
                </AuthButton>

                <AuthButton
                    backgroundColor={colors.accent2_300}
                    icon='logo-google'
                >
                    Continue with Google
                </AuthButton>
            </View>

            <View style={styles.signin}>
                <Text>Already have an account?</Text>
                <AuthLink
                    link='/login' 
                    style={{
                        color: colors.accent
                    }}> 
                    Sign in
                </AuthLink>
            </View>

        </View>

    )
}

const styles = StyleSheet.create({
    container: {
        display: 'flex',
        gap: spacing[2]
    },

    title: {
        gap: spacing[3]
    },

    policy: {
        display: 'flex',
        alignItems: 'center'
    },

    policy_text: {
        fontFamily: fontFamily.bodyRegular,
        fontSize: fontSize.secondary
    },

    signup_buttons: {
        gap: spacing[2]
    },

    signin: {
        flexDirection: 'row',
        justifyContent: 'center',
        gap: spacing[2],
        marginBottom: spacing[8]
    }
});