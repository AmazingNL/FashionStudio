import { Text, View, StyleSheet } from "react-native";
import { colors } from "../../theme";


export default function AuthDivider(){
    return(
        <View style={styles.container}>
            <View style={styles.line}></View>
                <Text style={{color : colors.divider}}>or</Text>
            <View style={styles.line}></View>
        </View>
    )
}

const styles = StyleSheet.create({
    line: {
        flex: 1,
        height: 1,
        backgroundColor: colors.divider,
    },

    container: {
        flexDirection: 'row',
        alignItems: 'center',
        gap: 20,
    }
})