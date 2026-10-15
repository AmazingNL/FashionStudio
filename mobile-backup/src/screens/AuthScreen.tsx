import React, { useState } from 'react';
import { StyleSheet, Text, View } from 'react-native';
import { Button, Screen, TextField } from '../components';
import { useAuth } from '../context/AuthContext';
import { ApiError } from '../api';
import { colors, fontFamily, fontSize, spacing } from '../theme';

export function AuthScreen() {
    const { signIn, signUp } = useAuth();
    const [mode, setMode] = useState<'login' | 'register'>('login');
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [userName, setUserName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    const isLogin = mode === 'login';

    async function handleSubmit() {
        setError(null);
        setLoading(true);
        try {
            if (isLogin) {
                await signIn(userName.trim(), password);
            } else {
                await signUp(firstName.trim(), lastName.trim(), userName.trim(), email.trim(), password);
            }
        } catch (err) {
            setError(err instanceof ApiError ? err.message : 'Something went wrong. Please try again.');
        } finally {
            setLoading(false);
        }
    }

    return (
        <Screen>
            <Text style={styles.title}>FashionStudio</Text>
            <Text style={styles.subtitle}>{isLogin ? 'Sign in to your studio' : 'Create your studio account'}</Text>

            <View style={styles.form}>
                {!isLogin && (
                    <>
                        <TextField label="First name" value={firstName} onChangeText={setFirstName} autoCapitalize="words" />
                        <TextField label="Last name" value={lastName} onChangeText={setLastName} autoCapitalize="words" />
                        <TextField
                            label="Email"
                            value={email}
                            onChangeText={setEmail}
                            autoCapitalize="none"
                            keyboardType="email-address"
                        />
                    </>
                )}
                <TextField label="Username" value={userName} onChangeText={setUserName} autoCapitalize="none" />
                <TextField label="Password" value={password} onChangeText={setPassword} secureTextEntry />

                {error && <Text style={styles.error}>{error}</Text>}

                <Button
                    label={isLogin ? 'Sign in' : 'Create account'}
                    onPress={handleSubmit}
                    loading={loading}
                    fullWidth
                />
                <Button
                    label={isLogin ? "Don't have an account? Sign up" : 'Already have an account? Sign in'}
                    onPress={() => {
                        setError(null);
                        setMode(isLogin ? 'register' : 'login');
                    }}
                    variant="secondary"
                    fullWidth
                />
            </View>
        </Screen>
    );
}

const styles = StyleSheet.create({
    title: { fontFamily: fontFamily.heading, fontSize: fontSize.screenTitle, color: colors.text, marginTop: spacing[8] },
    subtitle: { fontFamily: fontFamily.bodyRegular, fontSize: fontSize.body, color: colors.neutral700, marginBottom: spacing[4] },
    form: { gap: spacing[3] },
    error: { fontFamily: fontFamily.bodySemiBold, fontSize: fontSize.secondary, color: colors.accent700 },
});
