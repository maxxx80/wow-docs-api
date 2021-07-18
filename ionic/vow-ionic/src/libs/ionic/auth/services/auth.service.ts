import { Injectable } from '@angular/core';
import { AvailableResult, NativeBiometric } from 'capacitor-native-biometric';
import { v4 } from 'uuid';
import { SecureStoragePlugin } from 'capacitor-secure-storage-plugin';

const SECURITY_KEY = 'VOW_DOCS_SECURITY_KEY';

@Injectable()
export class AuthService {
    isAuthenticated = false;
    /*
    async tryEnter(): Promise<EnterSuccess> {
        const result = await NativeBiometric.isAvailable();
        const isAvailable = result.isAvailable;
        if (isAvailable) {
            try {
                // Authenticate using biometrics before logging the user in
                const identity = await NativeBiometric.verifyIdentity({
                    reason: 'Безопасный вход',
                    title: 'Для использования приложения необходимо авторизоваться',
                    subtitle: 'Данные авторизации будут проверены устройством',
                });

                // Get user's credentials
                const credentials = await NativeBiometric.getCredentials({
                    server: 'www.vow-docs.com',
                });

                console.log('credentials !!!', credentials);
                return credentials;
            } catch (err) {
                if (err.message === 'No credentials found') {
                    return this.setCredentials();
                } else {
                    console.warn('gate keeper error !!!', err);
                    return null;
                }
            }
        } else {
            console.warn('NativeBiometric is not available !!!');
            return null;
        }
    }

    setCredentials() {
        const username = v4();
        const password = v4();
        return NativeBiometric.setCredentials({
            server: 'www.vow-docs.com',
            username,
            password,
        });
    }
    */
    login() {}

    async getSecurityKey() {
        const result = await SecureStoragePlugin.get({ key: SECURITY_KEY });
        return result.value;
    }
}
