import Link from 'next/link'
import Layout from '../components/layout'
import withAuth from '../components/with-auth'

export default withAuth(
  (props) => (
    <Layout {...props}>
      <h1>Home</h1>
      <p>This is the homepage of the catalogue</p>
      {props.user && props.user.is_supplier &&
        <p><a href='/suppliers'>Go to supplier home</a></p>
      }
    </Layout>
  )
)
