// IMPORTANT — read this before running against your local backend.
//
// Your phone can't resolve "localhost" to your PC — on the phone, localhost
// means the phone itself. Two things have to be true for the app to reach
// FashionStudio.Api running on your machine:
//
// 1. Point this at your PC's LAN IP (the same one Expo showed you, e.g.
//    "exp://172.20.10.3:8081" -> use 172.20.10.3 here), not "localhost".
//    Set it via an env var so you don't have to edit committed code:
//      EXPO_PUBLIC_API_URL=http://172.20.10.3:5220 npm start
//    or put EXPO_PUBLIC_API_URL=... in a .env file in mobile/ (gitignored).
//
// 2. By default, Kestrel (the backend's dev server) only listens on
//    "localhost", which — same problem — isn't reachable from other devices.
//    Run the API with:
//      dotnet run --urls http://0.0.0.0:5220
//    and make sure Windows Firewall allows inbound connections on that port
//    (same class of issue we hit getting Expo Go to connect).
//
// If you're testing on the web target (npm run web) instead of a physical
// device, plain http://localhost:5220 works fine as-is.

export const API_BASE_URL = process.env.EXPO_PUBLIC_API_URL ?? 'http://localhost:5220';
