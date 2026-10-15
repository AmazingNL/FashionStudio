import { View, StyleSheet, ScrollView } from "react-native";
import { colors, spacing } from "../theme";
import SignupScreen from "../screens/SignupScreen";

export default function signup(){
    return(
        <ScrollView 
            style={styles.container}
            contentContainerStyle={{
                flexGrow: 1,
            }}
            keyboardShouldPersistTaps="handled"
            showsVerticalScrollIndicator={false}
            >
            <SignupScreen />
        </ScrollView>
    )
}

const styles = StyleSheet.create({
    container: {
        backgroundColor: colors.bg,
        padding: spacing[4],
        height: '100%',
    },


});