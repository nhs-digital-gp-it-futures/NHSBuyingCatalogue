import React from 'react'

export default class UserAuthenticationNav extends React.Component {
  render () {
    return this.props.user && this.props.user.is_authenticated
      ? (
        <div className='auth'>
          <span className='user'>
            <span className='sr'>
              You are logged in.
            </span>
            Hello, {this.props.user.first_name}
          </span>
          <span className='account'>
            <a href='/logout'>Your Account</a>
          </span>
        </div>
        )
      : (
        <div className='auth'>
          <span class='sr'>You are logged out.</span>
          <a href='/oidc/authenticate'>Log In</a>
        </div>
        )
  }
}
