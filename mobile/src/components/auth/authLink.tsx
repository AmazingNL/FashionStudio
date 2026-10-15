import { Link, Href } from 'expo-router';
import { ReactNode } from 'react';
import { TextStyle } from 'react-native';

interface AuthLinkProps {
    children : ReactNode
    link : Href
    style : TextStyle
}
export default function AuthLink({children, link, style}: AuthLinkProps){
    return(
        <Link 
            style={style}
            href={link}>
                {children}
        </Link>
    )
}