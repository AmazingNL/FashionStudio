import React from 'react';
import { colors, fontFamily, fontSize, spacing, radius, shadow  } from '../theme/index';
import { View, TextInput, StyleSheet, Text } from 'react-native';
import AuthHeader from '../components/auth/AuthHeader';
import AuthInput from '../components/auth/AuthInput';
import { Link } from 'expo-router';
import AuthLink from '../components/auth/authLink';
import AuthButton from '../components/auth/AuthButton';
import AuthDivider from '../components/auth/AthDivider';


export default function LoginScreen() {
  return (
    <View style={styles.container}>
      <AuthHeader />

      <View>
        <Text style={styles.welcome_back}>
          Welcome back
        </Text>
        <Text style={{fontSize: fontSize.secondary, color: colors.neutral800}}>Sign in continue to your studio</Text>
      </View>

      <View>
        <AuthInput 
          keyboardType="email-address" 
          placeholder='Enter your email address'
          icon="mail-outline"
          iconColor={colors.accent}
        />
        <AuthInput 
          placeholder='Enter your password'
          secureTextEntry 
          icon='key-outline'
          iconColor={colors.accent2}
        />
      </View>

      <View style={{alignItems: 'flex-end'}}>
          <AuthLink
            link='/forget_password'
            style={{
              fontSize: fontSize.secondary,  
              color: colors.accent, 
              fontFamily: fontFamily.bodyBold
            }}>
            Forgot password?
          </AuthLink>
      </View>

      <View>
        <AuthButton
          color={colors.bg}
          backgroundColor={colors.accent}
        >
          Sign in
        </AuthButton>
      </View>

      <View>
        <AuthDivider />
      </View>

      {/* Sign up section */}

      <View style={{gap: spacing[2]}}>
        <AuthButton 
          icon='logo-apple'
          backgroundColor={colors.neutral300}
          >
          Continue with Apple
        </AuthButton>

        <AuthButton 
          icon='logo-google'
          backgroundColor={colors.accent2_300}
          >
          Continue with Google
        </AuthButton>
      </View>

      <View style={styles.signup}>
        <Text>Don't have an account?</Text>
        <AuthLink
          link='/signup'
          style={{
            color: colors.accent
          }}
        >
          <Text>
            Sign up
          </Text>
        </AuthLink>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    display: 'flex',
    flex: 1,
    gap: 10
  },

  welcome_back : {
    color: colors.text, 
    fontSize: fontSize.sectionHeadingLg,
    fontFamily: fontFamily.bodyBold,
    marginBottom: 10
  },

  signup: {
    flexDirection: 'row',
    gap: spacing[2],
    alignItems: 'center',
    justifyContent: 'center'
    
  }
});