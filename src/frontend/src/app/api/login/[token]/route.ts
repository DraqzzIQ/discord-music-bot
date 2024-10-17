import { NextRequest, NextResponse } from 'next/server';

export async function GET(request: NextRequest, { params }: { params: { token: string } }) {
    const { token } = params;
    const url = new URL(process.env.NEXT_PUBLIC_BASE_URL);

    if (!token) {
        // Handle missing token (e.g., return an error response)
        return NextResponse.redirect(url);
    }

    // Set the token in an HTTP-only, secure cookie
    const response:NextResponse = NextResponse.redirect(url);
    response.cookies.set('authToken', token, {
        httpOnly: true,
        secure: true,
        sameSite: 'strict',
        path: '/',
        // 6 months from now
        expires: new Date(Date.now() + 1000 * 60 * 60 * 24 * 30 * 6)
    });

    return response;
}