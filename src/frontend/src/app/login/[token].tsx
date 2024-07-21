import { GetServerSideProps, NextPage } from 'next';

const LoginWithToken: NextPage = () => {
    return <div>Loading...</div>;
};

export const getServerSideProps: GetServerSideProps = async (context) => {
    const { token } = context.params as { token: string };

    // Set the token in an HTTP-only, secure cookie
    context.res.setHeader('Set-Cookie', `authToken=${token}; Path=/; HttpOnly; Secure; SameSite=Strict`);

    // Redirect to the homepage
    return {
        redirect: {
            destination: '/',
            permanent: false,
        },
    };
};

export default LoginWithToken;