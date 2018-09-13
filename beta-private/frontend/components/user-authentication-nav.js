import React from 'react'

export default class UserAuthenticationNav extends React.Component {
  render () {
    return this.props.user && this.props.user.is_authenticated
      ? (
        <div className='auth'>
          <span className='user'>Hello, {this.props.user.first_name}</span>
          <span className='account'>
            <a href='/logout'>Your Account</a>
          </span>
        </div>
        )
      : (
        <div className='auth'>
          <a href='/oidc/authenticate'>Log In</a>
        </div>
        )
  }
}
