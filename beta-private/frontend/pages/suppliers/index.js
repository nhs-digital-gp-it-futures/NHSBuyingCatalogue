import Layout from '../../components/layout'
import { suppliersOnly } from '../../components/with-auth'

export default suppliersOnly(
  (props) => (
    <Layout {...props} title='Supplier Home'>
      <h1>Supplier Home</h1>
      {
        props.user && props.user.org
          ? <p>You have logged in from supplier organisation {props.user.org.name}</p>
          : <p><strong>You should not be here!</strong></p>
      }
    </Layout>
  )
)
